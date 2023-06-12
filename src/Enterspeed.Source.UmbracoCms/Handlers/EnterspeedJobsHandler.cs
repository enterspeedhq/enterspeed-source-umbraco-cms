using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Factories;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Source.UmbracoCms.Handlers
{
    public class EnterspeedJobsHandler : IEnterspeedJobsHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly ILogger<EnterspeedJobsHandler> _logger;
        private readonly EnterspeedJobHandlerCollection _jobHandlers;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IEnterspeedPostJobsHandler _enterspeedPostJobsHandler;
        private static List<EnterspeedJob> NewFailedJobs => new();
        private static List<EnterspeedJob> ExistingFailedJobs => new();


        public EnterspeedJobsHandler(
            IEnterspeedJobRepository enterspeedJobRepository,
            ILogger<EnterspeedJobsHandler> logger,
            EnterspeedJobHandlerCollection jobHandlers,
            IEnterspeedJobFactory enterspeedJobFactory,
            IEnterspeedPostJobsHandler enterspeedPostJobsHandler)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _logger = logger;
            _jobHandlers = jobHandlers;
            _enterspeedJobFactory = enterspeedJobFactory;
            _enterspeedPostJobsHandler = enterspeedPostJobsHandler;
        }

        public void HandleJobs(IList<EnterspeedJob> jobsToProcess)
        {
            // Fetch all failed jobs for these content ids. We need to do this to delete the failed jobs if they no longer fails
            var failedJobsToHandle =
                _enterspeedJobRepository.GetFailedJobs(jobsToProcess.Select(x => x.EntityId).Distinct().ToList());

            var jobsByEntityIdAndContentState =
                jobsToProcess.GroupBy(x => new { x.EntityId, x.ContentState, x.Culture });

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

                var handler = _jobHandlers.FirstOrDefault(f => f.CanHandle(newestJob));
                if (handler == null)
                {
                    var message = $"No job handler available for {newestJob.EntityId} {newestJob.EntityType}";
                    NewFailedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestJob, message));
                    _logger.LogWarning(message);
                    continue;
                }

                try
                {
                    handler.Handle(newestJob);

                    // If job has been successfully handled and a failed job exists with same id, we should remove it
                    var existingFailedJobToDelete = failedJobsToHandle.Where(x =>
                        x.EntityId == newestJob.EntityId && x.Culture == newestJob.Culture &&
                        x.ContentState == newestJob.ContentState).ToList();

                    ExistingFailedJobs.AddRange(existingFailedJobToDelete);
                }
                catch (Exception exception)
                {
                    // Exceptions has a ToString() override which formats the full exception nicely.
                    var exceptionAsString = exception.ToString();
                    NewFailedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestJob, exceptionAsString));
                    _logger.LogError(exceptionAsString);
                }
            }

            _enterspeedPostJobsHandler.Handle(jobsToProcess, ExistingFailedJobs, NewFailedJobs);
        }
    }
}