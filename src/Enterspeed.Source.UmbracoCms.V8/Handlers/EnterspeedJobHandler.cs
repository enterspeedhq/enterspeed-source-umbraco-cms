using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers
{
    public class EnterspeedJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly ILogger _logger;
        private readonly IRedirectUrlService _redirectUrlService;
        private readonly IUmbracoUrlService _umbracoUrlService;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IUmbracoRedirectsService _redirectsService;

        public EnterspeedJobHandler(
            IEnterspeedJobRepository enterspeedJobRepository,
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedPropertyService enterspeedPropertyService,
            ILogger logger,
            IRedirectUrlService redirectUrlService,
            IUmbracoUrlService umbracoUrlService,
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IUmbracoRedirectsService redirectsService)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedPropertyService = enterspeedPropertyService;
            _logger = logger;
            _redirectUrlService = redirectUrlService;
            _umbracoUrlService = umbracoUrlService;
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _redirectsService = redirectsService;
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
                .Select(x => x.ContentId)
                .Distinct()
                .ToDictionary(x => x, x => jobs.Where(j => j.ContentId == x).Select(j => j.Culture).Distinct().ToList());

            // Fetch all failed jobs for these content ids. We need to do this to delete the failed jobs if they no longer fails
            var failedJobsToHandle = _enterspeedJobRepository.GetFailedJobs(jobs.Select(x => x.ContentId).Distinct().ToList());

            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var jobInfo in jobsToHandle)
                {
                    foreach (var culture in jobInfo.Value)
                    {
                        // List of all jobs with this contentid and culture
                        var jobsToRun = jobs
                            .Where(x => x.ContentId == jobInfo.Key && x.Culture == culture)
                            .OrderBy(x => x.CreatedAt)
                            .ToList();

                        // Get the failed jobs and add it to the batch of jobs that needs to be handled, so we can delete them afterwards
                        failedJobsToDelete.AddRange(failedJobsToHandle.Where(x =>
                            x.ContentId == jobInfo.Key && x.Culture == culture));

                        // We only need to execute the latest jobs instruction.
                        var newestJob = jobsToRun.LastOrDefault();
                        var shouldDelete = newestJob?.JobType == EnterspeedJobType.Delete;
                        var shouldPublish = newestJob?.JobType == EnterspeedJobType.Publish;

                        if (shouldPublish)
                        {
                            var content = context.UmbracoContext.Content.GetById(newestJob.ContentId);
                            if (content == null)
                            {
                                // Create a new failed job
                                var exception = $"Content with id {newestJob.ContentId} not in cache";
                                failedJobs.Add(GetFailedJob(newestJob, exception));
                                _logger.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }

                            // Create Umbraco Enterspeed Entity
                            UmbracoContentEntity umbracoData;
                            try
                            {
                                var redirects = _redirectsService.GetRedirects(content.Key, culture);
                                umbracoData = new UmbracoContentEntity(content, _enterspeedPropertyService, _entityIdentityService, redirects, culture);
                            }
                            catch (Exception e)
                            {
                                // Create a new failed job
                                var exception = $"Failed creating entity ({newestJob.ContentId}/{newestJob.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}";
                                failedJobs.Add(GetFailedJob(newestJob, exception));
                                _logger.Warn<EnterspeedJobHandler>(exception);
                                continue;
                            }

                            var ingestResponse = _enterspeedIngestService.Save(umbracoData);
                            if (!ingestResponse.Success)
                            {
                                // Create a new failed job
                                var message = ingestResponse.Exception != null
                                    ? ingestResponse.Exception.Message
                                    : ingestResponse.Message;

                                var exception =
                                    $"Failed ingesting entity ({newestJob.ContentId}/{newestJob.Culture}). Message: {message}";
                                failedJobs.Add(GetFailedJob(newestJob, exception));
                                _logger.Warn<EnterspeedJobHandler>(exception);
                            }
                        }
                        else if (shouldDelete)
                        {
                            var id = _entityIdentityService.GetId(newestJob.ContentId, newestJob.Culture);
                            var deleteResponse = _enterspeedIngestService.Delete(id);
                            if (!deleteResponse.Success)
                            {
                                // Create a new failed job
                                var exception = $"Failed deleting entity ({newestJob.ContentId}/{newestJob.Culture}). Message: {deleteResponse.Message}";
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
                ContentId = handledJob.ContentId,
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
