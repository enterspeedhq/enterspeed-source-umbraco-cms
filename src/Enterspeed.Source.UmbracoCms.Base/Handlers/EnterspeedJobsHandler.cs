using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Lucene.Net.Util;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Source.UmbracoCms.Base.Handlers
{
    public class EnterspeedJobsHandler : IEnterspeedJobsHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly ILogger<EnterspeedJobsHandler> _logger;
        private readonly EnterspeedJobHandlerCollection _jobHandlers;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IEnterspeedPostJobsHandler _enterspeedPostJobsHandler;

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

        /// <summary>
        /// Methods handle all incoming jobs to process. Umbraco Hosted service calls this implementation
        /// </summary>
        /// <param name="jobsToProcess"></param>
        public void HandleJobs(IList<EnterspeedJob> jobsToProcess)
        {
            var newFailedJobs = new List<EnterspeedJob>();
            var existingFailedJobsToDelete = new List<EnterspeedJob>();

            HandleJobs(jobsToProcess, newFailedJobs, existingFailedJobsToDelete);
            HandlePostJobs(jobsToProcess, newFailedJobs, existingFailedJobsToDelete);
        }

        private void HandleJobs(IList<EnterspeedJob> jobsToProcess,
            ICollection<EnterspeedJob> newFailedJobs, IList<EnterspeedJob> existingFailedJobsToDelete)
        {
            // Fetch all failed jobs for these content ids. We need to do this to delete the failed jobs if they no longer fails
            var failedJobsToHandle = _enterspeedJobRepository.GetFailedJobs(jobsToProcess.Select(x => x.EntityId).Distinct().ToList()).ToList();

            var jobsByEntityIdAndContentState = jobsToProcess.GroupBy(x => new { x.EntityId, x.ContentState, x.Culture });
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

                var handler = _jobHandlers.FirstOrDefault(f => f.CanHandle(jobToHandle));
                if (handler == null)
                {
                    var message = $"No job handler available for {jobToHandle.EntityId} {jobToHandle.EntityType}";
                    newFailedJobs.Add(_enterspeedJobFactory.GetFailedJob(jobToHandle, message));
                    _logger.LogWarning(message);
                    return;
                }

                HandleJob(handler, jobToHandle, failedJobsToHandle, newFailedJobs, existingFailedJobsToDelete);
            }
        }

        private void HandleJob(IEnterspeedJobHandler handler, EnterspeedJob jobToHandle,
            IEnumerable<EnterspeedJob> failedJobsToHandle, ICollection<EnterspeedJob> newFailedJobs,
            IList<EnterspeedJob> existingFailedJobsToDelete)
        {
            try
            {
                handler.Handle(jobToHandle);

                // If job has been successfully handled and a failed job exists with same id, we should remove it
                var existingFailedJobToDelete = failedJobsToHandle.Where(x =>
                    x.EntityId == jobToHandle.EntityId && x.Culture == jobToHandle.Culture &&
                    x.ContentState == jobToHandle.ContentState).ToList();

                existingFailedJobsToDelete.AddRange(existingFailedJobToDelete);
            }
            catch (Exception exception)
            {
                // Exceptions has a ToString() override which formats the full exception nicely.
                var exceptionAsString = exception.ToString();
                newFailedJobs.Add(_enterspeedJobFactory.GetFailedJob(jobToHandle, exceptionAsString));
                _logger.LogError(exceptionAsString);
            }
        }

        private void HandlePostJobs(IList<EnterspeedJob> jobsToProcess, IList<EnterspeedJob> newFailedJobs,
            IReadOnlyCollection<EnterspeedJob> existingFailedJobsToDelete)
        {
            _enterspeedPostJobsHandler.Handle(jobsToProcess, existingFailedJobsToDelete, newFailedJobs);
        }
    }
}