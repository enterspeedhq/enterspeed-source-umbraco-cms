using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Enterspeed.Source.UmbracoCms.V7.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V7.Extensions;
using Enterspeed.Source.UmbracoCms.V7.Factories;
using Enterspeed.Source.UmbracoCms.V7.Models;
using Enterspeed.Source.UmbracoCms.V7.Models.Api;
using Enterspeed.Source.UmbracoCms.V7.Services;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Enterspeed.Source.UmbracoCms.V7.Handlers
{
    public class EnterspeedJobHandler
    {
        private readonly EnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedIngestService _enterspeedPublishIngestService;
        private readonly IEnterspeedIngestService _enterspeedPreviewIngestService;
        private readonly EntityIdentityService _entityIdentityService;
        private readonly EnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedJobHandler()
        {
            _enterspeedJobRepository = EnterspeedContext.Current.Repositories.JobRepository;
            _enterspeedPublishIngestService = EnterspeedContext.Current.Services.PublishIngestService;
            _enterspeedPreviewIngestService = EnterspeedContext.Current.Services.PreviewIngestService;
            _entityIdentityService = EnterspeedContext.Current.Services.EntityIdentityService;
            _enterspeedJobFactory = EnterspeedContext.Current.Services.JobFactory;
        }

        public void HandlePendingJobs(int batchSize)
        {
            var jobCount = 1;
            while (jobCount > 0)
            {
                var jobs = _enterspeedJobRepository.GetPendingJobs(batchSize).ToList();
                jobCount = jobs.Count;
                try
                {
                    HandleJobs(jobs);
                }
                catch (Exception e)
                {
                    LogHelper.Error<EnterspeedJobHandler>("Failed handling pending jobs", e);
                }
            }
        }

        public void HandleJobs(List<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            LogHelper.Debug<EnterspeedJobHandler>("Handling {jobsCount} jobs", () => jobs.Count);

            // Update jobs from pending to processing
            foreach (var job in jobs)
            {
                job.State = EnterspeedJobState.Processing;
                job.UpdatedAt = DateTime.UtcNow;
            }

            _enterspeedJobRepository.Save(jobs);

            // Process nodes
            var failedJobs = new List<EnterspeedJob>();
            var failedJobsToDelete = new List<EnterspeedJob>();

            // Get a dictionary of contentid and cultures to handle
            var jobsToHandle = jobs
                .Select(x => x.EntityId)
                .Distinct()
                .ToDictionary(x => x, x => jobs.Where(j => j.EntityId == x).Select(j => j.Culture).Distinct().ToList());

            // Fetch all failed jobs for these content ids. We need to do this to delete the failed jobs if they no longer fails
            var failedJobsToHandle = _enterspeedJobRepository.GetFailedJobs(jobs.Select(x => x.EntityId).Distinct().ToList());
            var context = UmbracoContextHelper.GetUmbracoContext();
            foreach (var jobInfo in jobsToHandle)
            {
                foreach (var culture in jobInfo.Value)
                {
                    var jobsToRun = jobs
                        .Where(x => x.EntityId == jobInfo.Key && x.Culture == culture)
                        .ToList();

                    // List of all publish jobs with this contentid and culture
                    var publishJobsToRun = jobsToRun
                        .Where(x => x.ContentState == EnterspeedContentState.Publish)
                        .OrderBy(x => x.CreatedAt)
                        .ToList();

                    // List of all preview jobs with this contentid and culture
                    var previewJobsToRun = _enterspeedPreviewIngestService != null ? jobsToRun
                        .Where(x => x.ContentState == EnterspeedContentState.Preview)
                        .OrderBy(x => x.CreatedAt)
                        .ToList() : new List<EnterspeedJob>();

                    // Get the failed jobs and add it to the batch of jobs that needs to be handled, so we can delete them afterwards
                    failedJobsToDelete.AddRange(failedJobsToHandle.Where(x =>
                        x.EntityId == jobInfo.Key && x.Culture == culture));

                    // We only need to execute the latest jobs instruction.
                    var newestPublishJob = publishJobsToRun.LastOrDefault();
                    var newestPreviewJob = previewJobsToRun.LastOrDefault();
                    var shouldDeleteFromPublishSource = newestPublishJob?.JobType == EnterspeedJobType.Delete;
                    var shouldPublishToPublishSource = newestPublishJob?.JobType == EnterspeedJobType.Publish;
                    var shouldDeleteFromPreviewSource = newestPreviewJob?.JobType == EnterspeedJobType.Delete;
                    var shouldPublishToPreviewSource = newestPreviewJob?.JobType == EnterspeedJobType.Publish;

                    // Handle Publish source
                    if (shouldPublishToPublishSource)
                    {
                        IEnterspeedEntity umbracoData = null;
                        if (newestPublishJob.EntityType == EnterspeedJobEntityType.Content)
                        {
                            var isContentId = int.TryParse(newestPublishJob.EntityId, out var contentId);
                            var content = isContentId ? context.ContentCache.GetById(contentId) : null;
                            if (content == null)
                            {
                                // Create a new failed job
                                var exception = $"Content with id {newestPublishJob.EntityId} not in cache";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }

                            // Create Umbraco Enterspeed Entity
                            try
                            {
                                umbracoData = new UmbracoContentEntity(content);
                            }
                            catch (Exception e)
                            {
                                // Create a new failed job
                                var exception = $"Failed creating entity ({newestPublishJob.EntityId}/{newestPublishJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }
                        }
                        else if (newestPublishJob.EntityType == EnterspeedJobEntityType.Dictionary)
                        {
                            var isDictionaryId = Guid.TryParse(newestPublishJob.EntityId, out var dictionaryId);
                            var dictionaryItem = isDictionaryId ? context.Application.Services.LocalizationService.GetDictionaryItemById(dictionaryId) : null;
                            if (dictionaryItem == null)
                            {
                                // Create a new failed job
                                var exception = $"Dictionary with id {newestPublishJob.EntityId} not in database";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }

                            // Create Umbraco Enterspeed Entity
                            try
                            {
                                umbracoData = new UmbracoDictionaryEntity(dictionaryItem, culture);
                            }
                            catch (Exception e)
                            {
                                // Create a new failed job
                                var exception = $"Failed creating entity ({newestPublishJob.EntityId}/{newestPublishJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }
                        }
                        else if (newestPublishJob.EntityType == EnterspeedJobEntityType.Media)
                        {
                            var parsed = int.TryParse(newestPublishJob.EntityId, out var parsedId);
                            var media = parsed ? context.Application.Services.MediaService.GetById(parsedId) : null;

                            if (media == null)
                            {
                                var exception = $"Media with id {newestPublishJob.EntityId} not in database";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                            }

                            try
                            {
                                umbracoData = new UmbracoMediaEntity(media);
                            }
                            catch (Exception e)
                            {
                                // Create a new failed job
                                var exception = $"Failed creating entity ({newestPublishJob.EntityId}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }
                        }

                        var ingestResponse = _enterspeedPublishIngestService.Save(umbracoData);
                        if (!ingestResponse.Success)
                        {
                            // Create a new failed job
                            var message = Json.Encode(new ErrorResponse(ingestResponse));

                            var exception = $"Failed ingesting entity ({newestPublishJob.EntityId}/{newestPublishJob.Culture}). Message: {message}";
                            failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                            LogHelper.Warn<EnterspeedJobHandler>(exception);
                        }
                    }
                    else if (shouldDeleteFromPublishSource)
                    {
                        var id = string.Empty;
                        if (newestPublishJob.EntityType == EnterspeedJobEntityType.Content)
                        {
                            id = _entityIdentityService.GetId(newestPublishJob.EntityId);
                        }
                        else if (newestPublishJob.EntityType == EnterspeedJobEntityType.Dictionary)
                        {
                            id = _entityIdentityService.GetId(newestPublishJob.EntityId, newestPublishJob.Culture);
                        }
                        else if (newestPublishJob.EntityType == EnterspeedJobEntityType.Media)
                        {
                            id = _entityIdentityService.GetId(newestPublishJob.EntityId);
                        }

                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            var deleteResponse = _enterspeedPublishIngestService.Delete(id);
                            if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
                            {
                                // Create a new failed job
                                var exception = $"Failed deleting entity ({newestPublishJob.EntityId}/{newestPublishJob.Culture}). Message: {deleteResponse.Message}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }
                        }
                    }

                    // Handle Preview source
                    if (shouldPublishToPreviewSource)
                    {
                        IEnterspeedEntity umbracoData = null;
                        if (newestPreviewJob.EntityType == EnterspeedJobEntityType.Content)
                        {
                            var isContentId = int.TryParse(newestPreviewJob.EntityId, out var contentId);
                            var content = isContentId ? ApplicationContext.Current.Services.ContentService.GetById(contentId).ToPublishedContent() : null;
                            if (content == null)
                            {
                                // Create a new failed job
                                var exception = $"Content with id {newestPreviewJob.EntityId} not in cache";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }

                            // Create Umbraco Enterspeed Entity
                            try
                            {
                                umbracoData = new UmbracoContentEntity(content);
                            }
                            catch (Exception e)
                            {
                                // Create a new failed job
                                var exception = $"Failed creating entity ({newestPreviewJob.EntityId}/{newestPreviewJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }
                        }
                        else if (newestPreviewJob.EntityType == EnterspeedJobEntityType.Dictionary)
                        {
                            var isDictionaryId = Guid.TryParse(newestPreviewJob.EntityId, out var dictionaryId);
                            var dictionaryItem = isDictionaryId ? context.Application.Services.LocalizationService.GetDictionaryItemById(dictionaryId) : null;
                            if (dictionaryItem == null)
                            {
                                // Create a new failed job
                                var exception = $"Dictionary with id {newestPreviewJob.EntityId} not in database";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }

                            // Create Umbraco Enterspeed Entity
                            try
                            {
                                umbracoData = new UmbracoDictionaryEntity(dictionaryItem, culture);
                            }
                            catch (Exception e)
                            {
                                // Create a new failed job
                                var exception = $"Failed creating entity ({newestPreviewJob.EntityId}/{newestPreviewJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }
                        }
                        else if (newestPreviewJob.EntityType == EnterspeedJobEntityType.Media)
                        {
                            var parsed = int.TryParse(newestPreviewJob.EntityId, out var parsedId);
                            var media = parsed ? context.Application.Services.MediaService.GetById(parsedId) : null;

                            if (media == null)
                            {
                                var exception = $"Media with id {newestPreviewJob.EntityId} not in database";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                            }

                            try
                            {
                                umbracoData = new UmbracoMediaEntity(media);
                            }
                            catch (Exception e)
                            {
                                // Create a new failed job
                                var exception = $"Failed creating entity ({newestPublishJob.EntityId}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }
                        }

                        var ingestResponse = _enterspeedPreviewIngestService.Save(umbracoData);
                        if (!ingestResponse.Success)
                        {
                            // Create a new failed job
                            var message = Json.Encode(new ErrorResponse(ingestResponse));

                            var exception = $"Failed ingesting entity ({newestPreviewJob.EntityId}/{newestPreviewJob.Culture}). Message: {message}";
                            failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                            LogHelper.Warn<EnterspeedJobHandler>(exception);
                        }
                    }
                    else if (shouldDeleteFromPreviewSource)
                    {
                        var id = string.Empty;
                        if (newestPreviewJob.EntityType == EnterspeedJobEntityType.Content)
                        {
                            id = _entityIdentityService.GetId(newestPreviewJob.EntityId);
                        }
                        else if (newestPreviewJob.EntityType == EnterspeedJobEntityType.Dictionary)
                        {
                            id = _entityIdentityService.GetId(newestPreviewJob.EntityId, newestPreviewJob.Culture);
                        }
                        else if (newestPreviewJob.EntityType == EnterspeedJobEntityType.Media)
                        {
                            id = _entityIdentityService.GetId(newestPreviewJob.EntityId, newestPreviewJob.Culture);
                        }

                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            var deleteResponse = _enterspeedPreviewIngestService.Delete(id);
                            if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
                            {
                                // Create a new failed job
                                var exception = $"Failed deleting entity ({newestPreviewJob.EntityId}/{newestPreviewJob.Culture}). Message: {deleteResponse.Message}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                LogHelper.Warn<EnterspeedJobHandler>(exception);
                            }
                        }
                    }
                }
            }

            // Save all jobs that failed
            _enterspeedJobRepository.Save(failedJobs);

            // Delete all jobs - Note, that it's safe to delete all jobs because failed jobs will be created as a new job
            _enterspeedJobRepository.Delete(jobs.Select(x => x.Id).Concat(failedJobsToDelete.Select(x => x.Id)).ToList());

            // Throw exception with a combined exception message for all jobs that failed if any
            if (failedJobs.Any())
            {
                var failedJobExceptions = string.Join(Environment.NewLine, failedJobs.Select(x => x.Exception));
                throw new Exception(failedJobExceptions);
            }
        }

        public void InvalidateOldProcessingJobs()
        {
            var oldJobs = _enterspeedJobRepository.GetOldProcessingTasks().ToList();
            if (oldJobs.Any())
            {
                foreach (var job in oldJobs)
                {
                    job.State = EnterspeedJobState.Failed;
                    job.Exception = $"Job processing timed out. Last updated at: {job.UpdatedAt}";
                    job.UpdatedAt = DateTime.UtcNow;
                }

                _enterspeedJobRepository.Save(oldJobs);
            }
        }
    }
}
