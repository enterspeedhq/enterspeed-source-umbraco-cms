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
        private static List<EnterspeedJob> ExistingFailedJobsToDelete => new();

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
            // TODO: Not a fan of assigning this variable here and parsing it around. Refactor?
            var failedJobsToHandle =
                _enterspeedJobRepository.GetFailedJobs(jobsToProcess.Select(x => x.EntityId).Distinct().ToList());

            HandleJobs(jobsToProcess, failedJobsToHandle);
            HandlePostJobs(jobsToProcess);
        }

        private void HandleJobs(IEnumerable<EnterspeedJob> jobsToProcess, IList<EnterspeedJob> failedJobsToHandle)
        {
            var jobsByEntityIdAndContentState =
                jobsToProcess.GroupBy(x => new { x.EntityId, x.ContentState, x.Culture });

            foreach (var jobInfo in jobsByEntityIdAndContentState)
            {
                // Get newest job to handle
                var jobToHandle = jobInfo
                    .OrderBy(x => x.CreatedAt)
                    .LastOrDefault();

                // We only need to execute the latest jobs instruction.
                if (jobToHandle == null)
                {
                    continue;
                }

                HandleJob(jobToHandle, failedJobsToHandle);
            }
        }

        private void HandleJob(EnterspeedJob jobToHandle, IList<EnterspeedJob> failedJobsToHandle)
        {
            var handler = _jobHandlers.FirstOrDefault(f => f.CanHandle(jobToHandle));
            if (handler == null)
            {
                var message = $"No job handler available for {jobToHandle.EntityId} {jobToHandle.EntityType}";
                NewFailedJobs.Add(_enterspeedJobFactory.GetFailedJob(jobToHandle, message));
                _logger.LogWarning(message);
                return;
            }

            HandleJob(handler, jobToHandle, failedJobsToHandle);
        }

        private void HandleJob(IEnterspeedJobHandler handler, EnterspeedJob jobToHandle,
            IEnumerable<EnterspeedJob> failedJobsToHandle)
        {
            try
            {
                handler.Handle(jobToHandle);

                // If job has been successfully handled and a failed job exists with same id, we should remove it
                var existingFailedJobToDelete = failedJobsToHandle.Where(x =>
                    x.EntityId == jobToHandle.EntityId && x.Culture == jobToHandle.Culture &&
                    x.ContentState == jobToHandle.ContentState).ToList();

                ExistingFailedJobsToDelete.AddRange(existingFailedJobToDelete);
            }
            catch (Exception exception)
            {
                // Exceptions has a ToString() override which formats the full exception nicely.
                var exceptionAsString = exception.ToString();
                NewFailedJobs.Add(_enterspeedJobFactory.GetFailedJob(jobToHandle, exceptionAsString));
                _logger.LogError(exceptionAsString);
            }
        }

        private void HandlePostJobs(IList<EnterspeedJob> jobsToProcess)
        {
            _enterspeedPostJobsHandler.Handle(jobsToProcess, ExistingFailedJobsToDelete, NewFailedJobs);
        }
    }
}