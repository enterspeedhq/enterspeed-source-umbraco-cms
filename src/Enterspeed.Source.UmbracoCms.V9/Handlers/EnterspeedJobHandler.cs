using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Factories;
using Enterspeed.Source.UmbracoCms.V9.Models;
using Enterspeed.Source.UmbracoCms.V9.Providers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
{
    public class EnterspeedJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly ILogger<EnterspeedJobHandler> _logger;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IUmbracoRedirectsService _redirectsService;
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedGuardService _enterspeedGuardService;
        private readonly IEnterspeedConnection _publishConnection;
        private readonly IEnterspeedConnection _previewConnection;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUrlFactory _urlFactory;

        public EnterspeedJobHandler(
            IEnterspeedJobRepository enterspeedJobRepository,
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedPropertyService enterspeedPropertyService,
            ILogger<EnterspeedJobHandler> logger,
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IUmbracoRedirectsService redirectsService,
            ILocalizationService localizationService,
            IEnterspeedGuardService enterspeedGuardService,
            IEnterspeedConnectionProvider connectionProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IUrlFactory urlFactory)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedPropertyService = enterspeedPropertyService;
            _logger = logger;
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _redirectsService = redirectsService;
            _localizationService = localizationService;
            _enterspeedGuardService = enterspeedGuardService;
            _enterspeedJobFactory = enterspeedJobFactory;
            _urlFactory = urlFactory;
            _publishConnection = connectionProvider.GetConnection(ConnectionType.Publish);
            _previewConnection = connectionProvider.GetConnection(ConnectionType.Preview);
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
                    _logger.LogError(e, "Error has happened");
                }
            }
        }

        public void HandleJobs(List<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            _logger.LogDebug("Handling {jobsCount} jobs", jobs.Count);

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
            var failedJobsToHandle =
                _enterspeedJobRepository.GetFailedJobs(jobs.Select(x => x.EntityId).Distinct().ToList());

            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var jobInfo in jobsToHandle)
                {
                    foreach (var culture in jobInfo.Value)
                    {
                        // List of all jobs with this contentid and culture
                        var jobsToRun = jobs
                            .Where(x => x.EntityId == jobInfo.Key && x.Culture == culture)
                            .OrderBy(x => x.CreatedAt)
                            .ToList();

                        // List of all publish jobs with this contentid and culture
                        var publishJobsToRun = jobsToRun
                            .Where(x => x.ContentState == EnterspeedContentState.Publish)
                            .OrderBy(x => x.CreatedAt)
                            .ToList();

                        // List of all preview jobs with this contentid and culture
                        var previewJobsToRun = _previewConnection != null ? jobsToRun
                            .Where(x => x.ContentState == EnterspeedContentState.Preview)
                            .OrderBy(x => x.CreatedAt)
                            .ToList() : new List<EnterspeedJob>();

                        // Get the failed jobs and add it to the batch of jobs that needs to be handled, so we can delete them afterwards
                        failedJobsToDelete.AddRange(failedJobsToHandle.Where(x =>
                            x.EntityId == jobInfo.Key && x.Culture == culture));

                        // We only need to execute the latest jobs instruction.
                        var newestPublishJob = publishJobsToRun.LastOrDefault();
                        var newestPreviewJob = previewJobsToRun.LastOrDefault();
                        var shouldDeleteFromPublishSource = _publishConnection != null && newestPublishJob?.JobType == EnterspeedJobType.Delete;
                        var shouldPublishToPublishSource = _publishConnection != null && newestPublishJob?.JobType == EnterspeedJobType.Publish;
                        var shouldDeleteFromPreviewSource = _previewConnection != null && newestPreviewJob?.JobType == EnterspeedJobType.Delete;
                        var shouldPublishToPreviewSource = _previewConnection != null && newestPreviewJob?.JobType == EnterspeedJobType.Publish;

                        if (shouldPublishToPublishSource)
                        {
                            IEnterspeedEntity umbracoData = null;

                            if (newestPublishJob.EntityType == EnterspeedJobEntityType.Content)
                            {
                                var isContentId = int.TryParse(newestPublishJob.EntityId, out var contentId);
                                var content = isContentId ? context.UmbracoContext.Content.GetById(contentId) : null;
                                if (content == null)
                                {
                                    // Create a new failed job
                                    var exception = $"Content with id {newestPublishJob.EntityId} not in cache";
                                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                    _logger.LogWarning(exception);
                                    continue;
                                }

                                // Check if any of guards are against it
                                if (!_enterspeedGuardService.CanIngest(content, culture))
                                {
                                    // Skip it, if is not valid.
                                    continue;
                                }

                                // Create Umbraco Enterspeed Entity
                                try
                                {
                                    var redirects = _redirectsService.GetRedirects(content.Key, culture);
                                    umbracoData = new UmbracoContentEntity(
                                        content, _enterspeedPropertyService, _entityIdentityService, redirects,
                                        _urlFactory, culture);
                                }
                                catch (Exception e)
                                {
                                    // Create a new failed job
                                    var exception =
                                        $"Failed creating entity ({newestPublishJob.EntityId}/{newestPublishJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                    _logger.LogWarning(exception);
                                    continue;
                                }
                            }
                            else if (newestPublishJob.EntityType == EnterspeedJobEntityType.Dictionary)
                            {
                                var isDictionaryId = Guid.TryParse(newestPublishJob.EntityId, out var dictionaryId);
                                var dictionaryItem = isDictionaryId
                                    ? _localizationService.GetDictionaryItemById(dictionaryId)
                                    : null;
                                if (dictionaryItem == null)
                                {
                                    // Create a new failed job
                                    var exception = $"Dictionary with id {newestPublishJob.EntityId} not in database";
                                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                    _logger.LogWarning(exception);
                                    continue;
                                }

                                // Check if any of guards are against it
                                if (!_enterspeedGuardService.CanIngest(dictionaryItem, culture))
                                {
                                    // Skip it, if is not valid.
                                    continue;
                                }

                                // Create Umbraco Enterspeed Entity
                                try
                                {
                                    umbracoData = new UmbracoDictionaryEntity(
                                        dictionaryItem, _enterspeedPropertyService, _entityIdentityService, culture);
                                }
                                catch (Exception e)
                                {
                                    // Create a new failed job
                                    var exception =
                                        $"Failed creating entity ({newestPublishJob.EntityId}/{newestPublishJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                    _logger.LogWarning(exception);
                                    continue;
                                }
                            }

                            var ingestResponse = _enterspeedIngestService.Save(umbracoData, _publishConnection);
                            if (!ingestResponse.Success)
                            {
                                // Create a new failed job
                                var message = ingestResponse.Exception != null
                                    ? ingestResponse.Exception.Message
                                    : ingestResponse.Message;

                                var exception =
                                    $"Failed ingesting entity ({newestPublishJob.EntityId}/{newestPublishJob.Culture}). Message: {message}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                _logger.LogWarning(exception);
                            }
                        }
                        else if (shouldDeleteFromPublishSource)
                        {
                            var id = _entityIdentityService.GetId(newestPublishJob.EntityId, newestPublishJob.Culture);
                            var deleteResponse = _enterspeedIngestService.Delete(id, _publishConnection);
                            if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
                            {
                                // Create a new failed job
                                var exception =
                                    $"Failed deleting entity ({newestPublishJob.EntityId}/{newestPublishJob.Culture}). Message: {deleteResponse.Message}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPublishJob, exception));
                                _logger.LogWarning(exception);
                                continue;
                            }
                        }

                        if (shouldPublishToPreviewSource)
                        {
                            IEnterspeedEntity umbracoData = null;

                            if (newestPreviewJob.EntityType == EnterspeedJobEntityType.Content)
                            {
                                var isContentId = int.TryParse(newestPreviewJob.EntityId, out var contentId);
                                var content = isContentId ? context.UmbracoContext.Content.GetById(true, contentId) : null;
                                if (content == null)
                                {
                                    // Create a new failed job
                                    var exception = $"Content with id {newestPreviewJob.EntityId} not in cache";
                                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                    _logger.LogWarning(exception);
                                    continue;
                                }

                                // Check if any of guards are against it
                                if (!_enterspeedGuardService.CanIngest(content, culture))
                                {
                                    // Skip it, if is not valid.
                                    continue;
                                }

                                // Create Umbraco Enterspeed Entity
                                try
                                {
                                    var redirects = _redirectsService.GetRedirects(content.Key, culture);
                                    umbracoData = new UmbracoContentEntity(
                                        content, _enterspeedPropertyService, _entityIdentityService, redirects,
                                        _urlFactory, culture);
                                }
                                catch (Exception e)
                                {
                                    // Create a new failed job
                                    var exception =
                                        $"Failed creating entity ({newestPreviewJob.EntityId}/{newestPreviewJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                    _logger.LogWarning(exception);
                                    continue;
                                }
                            }
                            else if (newestPreviewJob.EntityType == EnterspeedJobEntityType.Dictionary)
                            {
                                var isDictionaryId = Guid.TryParse(newestPreviewJob.EntityId, out var dictionaryId);
                                var dictionaryItem = isDictionaryId
                                    ? _localizationService.GetDictionaryItemById(dictionaryId)
                                    : null;
                                if (dictionaryItem == null)
                                {
                                    // Create a new failed job
                                    var exception = $"Dictionary with id {newestPreviewJob.EntityId} not in database";
                                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                    _logger.LogWarning(exception);
                                    continue;
                                }

                                // Check if any of guards are against it
                                if (!_enterspeedGuardService.CanIngest(dictionaryItem, culture))
                                {
                                    // Skip it, if is not valid.
                                    continue;
                                }

                                // Create Umbraco Enterspeed Entity
                                try
                                {
                                    umbracoData = new UmbracoDictionaryEntity(
                                        dictionaryItem, _enterspeedPropertyService, _entityIdentityService, culture);
                                }
                                catch (Exception e)
                                {
                                    // Create a new failed job
                                    var exception =
                                        $"Failed creating entity ({newestPreviewJob.EntityId}/{newestPreviewJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                    _logger.LogWarning(exception);
                                    continue;
                                }
                            }

                            var ingestResponse = _enterspeedIngestService.Save(umbracoData, _previewConnection);
                            if (!ingestResponse.Success)
                            {
                                // Create a new failed job
                                var message = ingestResponse.Exception != null
                                    ? ingestResponse.Exception.Message
                                    : ingestResponse.Message;

                                var exception =
                                    $"Failed ingesting entity ({newestPreviewJob.EntityId}/{newestPreviewJob.Culture}). Message: {message}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                _logger.LogWarning(exception);
                            }
                        }
                        else if (shouldDeleteFromPreviewSource)
                        {
                            var id = _entityIdentityService.GetId(newestPreviewJob.EntityId, newestPreviewJob.Culture);
                            var deleteResponse = _enterspeedIngestService.Delete(id, _previewConnection);
                            if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
                            {
                                // Create a new failed job
                                var exception =
                                    $"Failed deleting entity ({newestPreviewJob.EntityId}/{newestPreviewJob.Culture}). Message: {deleteResponse.Message}";
                                failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestPreviewJob, exception));
                                _logger.LogWarning(exception);
                                continue;
                            }
                        }
                    }
                }
            }

            // Save all jobs that failed
            _enterspeedJobRepository.Save(failedJobs);

            // Delete all jobs - Note, that it's safe to delete all jobs because failed jobs will be created as a new job
            _enterspeedJobRepository.Delete(
                jobs.Select(x => x.Id).Concat(failedJobsToDelete.Select(x => x.Id)).ToList());

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