using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Routing;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers
{
    public class EnterspeedJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly ILogger _logger;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IUmbracoRedirectsService _redirectsService;
        private readonly IPublishedRouter _publishedRouter;
        public EnterspeedJobHandler(
            IEnterspeedJobRepository enterspeedJobRepository,
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedPropertyService enterspeedPropertyService,
            ILogger logger,
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IUmbracoRedirectsService redirectsService,
            IPublishedRouter publishedRouter)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedPropertyService = enterspeedPropertyService;
            _logger = logger;
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _redirectsService = redirectsService;
            _publishedRouter = publishedRouter;
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
                    _logger.Error<EnterspeedJobHandler>(e);
                }
            }
        }

        public void HandleJobs(List<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            _logger.Debug<EnterspeedJobHandler>("Handling {jobsCount} jobs", jobs.Count);

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

                        // Get the failed jobs and add it to the batch of jobs that needs to be handled, so we can delete them afterwards
                        failedJobsToDelete.AddRange(failedJobsToHandle.Where(x =>
                            x.EntityId == jobInfo.Key && x.Culture == culture));

                        // We only need to execute the latest jobs instruction.
                        var newestJob = jobsToRun.LastOrDefault();
                        var shouldDelete = newestJob?.JobType == EnterspeedJobType.Delete;
                        var shouldPublish = newestJob?.JobType == EnterspeedJobType.Publish;

                        if (shouldPublish)
                        {
                            IEnterspeedEntity umbracoData = null;

                            if (newestJob.EntityType == EnterspeedJobEntityType.Content)
                            {
                                var isContentId = int.TryParse(newestJob.EntityId, out var contentId);
                                var content = isContentId ? context.UmbracoContext.Content.GetById(contentId) : null;
                                if (content == null)
                                {
                                    // Create a new failed job
                                    var exception = $"Content with id {newestJob.EntityId} not in cache";
                                    failedJobs.Add(GetFailedJob(newestJob, exception));
                                    _logger.Warn<EnterspeedJobHandler>(exception);
                                    continue;
                                }

                                // Create Umbraco Enterspeed Entity
                                try
                                {
                                    var redirects = _redirectsService.GetRedirects(content.Key, culture);
                                    Current.UmbracoContext.PublishedRequest = _publishedRouter.CreateRequest(context.UmbracoContext);
                                    Current.UmbracoContext.PublishedRequest.PublishedContent = content;
                                    umbracoData = new UmbracoContentEntity(content, _enterspeedPropertyService, _entityIdentityService, redirects, culture);
                                }
                                catch (Exception e)
                                {
                                    // Create a new failed job
                                    var exception = $"Failed creating entity ({newestJob.EntityId}/{newestJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                    failedJobs.Add(GetFailedJob(newestJob, exception));
                                    _logger.Warn<EnterspeedJobHandler>(exception);
                                    continue;
                                }
                            }
                            else if (newestJob.EntityType == EnterspeedJobEntityType.Dictionary)
                            {
                                var isDictionaryId = Guid.TryParse(newestJob.EntityId, out var dictionaryId);
                                var dictionaryItem = isDictionaryId ? Current.Services.LocalizationService.GetDictionaryItemById(dictionaryId) : null;
                                if (dictionaryItem == null)
                                {
                                    // Create a new failed job
                                    var exception = $"Dictionary with id {newestJob.EntityId} not in database";
                                    failedJobs.Add(GetFailedJob(newestJob, exception));
                                    _logger.Warn<EnterspeedJobHandler>(exception);
                                    continue;
                                }

                                // Create Umbraco Enterspeed Entity
                                try
                                {
                                    umbracoData = new UmbracoDictionaryEntity(dictionaryItem, _enterspeedPropertyService, _entityIdentityService, culture);
                                }
                                catch (Exception e)
                                {
                                    // Create a new failed job
                                    var exception = $"Failed creating entity ({newestJob.EntityId}/{newestJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                    failedJobs.Add(GetFailedJob(newestJob, exception));
                                    _logger.Warn<EnterspeedJobHandler>(exception);
                                    continue;
                                }
                            }

                            var ingestResponse = _enterspeedIngestService.Save(umbracoData);
                            if (!ingestResponse.Success)
                            {
                                // Create a new failed job
                                var message = ingestResponse.Exception != null
                                    ? ingestResponse.Exception.Message
                                    : ingestResponse.Message;

                                var exception =
                                    $"Failed ingesting entity ({newestJob.EntityId}/{newestJob.Culture}). Message: {message}";
                                failedJobs.Add(GetFailedJob(newestJob, exception));
                                _logger.Warn<EnterspeedJobHandler>(exception);
                            }
                        }
                        else if (shouldDelete)
                        {
                            var id = _entityIdentityService.GetId(newestJob.EntityId, newestJob.Culture);
                            var deleteResponse = _enterspeedIngestService.Delete(id);
                            if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
                            {
                                // Create a new failed job
                                var exception = $"Failed deleting entity ({newestJob.EntityId}/{newestJob.Culture}). Message: {deleteResponse.Message}";
                                failedJobs.Add(GetFailedJob(newestJob, exception));
                                _logger.Warn<EnterspeedJobHandler>(exception);
                                continue;
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

        private EnterspeedJob GetFailedJob(EnterspeedJob handledJob, string exception)
        {
            return new EnterspeedJob
            {
                EntityId = handledJob.EntityId,
                EntityType = handledJob.EntityType,
                Culture = handledJob.Culture,
                CreatedAt = handledJob.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                JobType = handledJob.JobType,
                State = EnterspeedJobState.Failed,
                Exception = exception
            };
        }
    }
}
