using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Factories;
using Umbraco.Core.Logging;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers
{
    public class EnterspeedJobsHandler : IEnterspeedJobsHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly ILogger _logger;
        private readonly EnterspeedJobHandlerCollection _jobHandlers;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedJobsHandler(
            IEnterspeedJobRepository enterspeedJobRepository,
            ILogger logger,
            EnterspeedJobHandlerCollection jobHandlers,
            IEnterspeedJobFactory enterspeedJobFactory)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _logger = logger;
            _jobHandlers = jobHandlers;
            _enterspeedJobFactory = enterspeedJobFactory;
        }

        public void HandleJobs(IList<EnterspeedJob> jobs)
        {
            // Process nodes
            var failedJobs = new List<EnterspeedJob>();
            var failedJobsToDelete = new List<EnterspeedJob>();

            // Fetch all failed jobs for these content ids. We need to do this to delete the failed jobs if they no longer fails
            var failedJobsToHandle =
                _enterspeedJobRepository.GetFailedJobs(jobs.Select(x => x.EntityId).Distinct().ToList());
            var jobsByEntityIdAndContentState = jobs.GroupBy(x => new { x.EntityId, x.ContentState, x.Culture });
            foreach (var jobInfo in jobsByEntityIdAndContentState)
            {
                var newestJob = jobInfo
                    .OrderBy(x => x.CreatedAt)
                    .LastOrDefault();

                // We only need to execute the latest jobs instruction.
                if (newestJob == null)
                {
                    continue;
                }

                // Get the failed jobs and add it to the batch of jobs that needs to be handled, so we can delete them afterwards
                failedJobsToDelete.AddRange(
                    failedJobsToHandle.Where(
                        x =>
                            x.EntityId == jobInfo.Key.EntityId && x.Culture == jobInfo.Key.Culture && x.ContentState == jobInfo.Key.ContentState));

                var handler = _jobHandlers.FirstOrDefault(f => f.CanHandle(newestJob));
                if (handler == null)
                {
                    var message = $"No job handler available for {newestJob.EntityId} {newestJob.EntityType}";
                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestJob, message));
                    _logger.Warn<EnterspeedJobsHandler>(message);
                    continue;
                }

                try
                {
                    handler.Handle(newestJob);
                }
                catch (Exception exception)
                {
                    var message = exception?.Message ?? "Failed to handle the job";
                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestJob, message));
                    _logger.Warn<EnterspeedJobsHandler>(message);
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
    }
}