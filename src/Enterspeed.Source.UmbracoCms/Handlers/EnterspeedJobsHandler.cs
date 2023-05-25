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

        public EnterspeedJobsHandler(
            IEnterspeedJobRepository enterspeedJobRepository,
            ILogger<EnterspeedJobsHandler> logger,
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

            // Fetch all failed jobs for these content ids. We need to do this to delete the failed jobs if they no longer fails
            var failedJobsToHandle =
                _enterspeedJobRepository.GetFailedJobs(jobs.Select(x => x.EntityId).Distinct().ToList());
            var failedJobsToDelete = new List<EnterspeedJob>();

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

                var handler = _jobHandlers.FirstOrDefault(f => f.CanHandle(newestJob));
                if (handler == null)
                {
                    var message = $"No job handler available for {newestJob.EntityId} {newestJob.EntityType}";
                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestJob, message));
                    _logger.LogWarning(message);
                    continue;
                }

                try
                {
                    handler.Handle(newestJob);

                    // If job has been successfully handled and a failed job exists with same id, we should remove it
                    var existingFailedJob = failedJobsToHandle.FirstOrDefault(fj => fj.Id == newestJob.Id);
                    if (existingFailedJob != null)
                    {
                        failedJobsToDelete.Add(newestJob);
                    }
                }
                catch (Exception exception)
                {
                    // Exceptions has a ToString() override which formats the full exception nicely.
                    var exceptionAsString = exception.ToString();
                    failedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestJob, exceptionAsString));
                    _logger.LogError(exceptionAsString);
                }
            }

            // Remove existing failed jobs that was handled
            RemoveFromFailedJobs(failedJobsToDelete);

            // Save existing and new failed jobs
            SaveFailedJobs(failedJobs);

            // Throw exception with a combined exception message for all jobs that failed if any
            if (failedJobs.Any())
            {
                var failedJobExceptions = string.Join(Environment.NewLine, failedJobs.Select(x => x.Exception));
                throw new Exception(failedJobExceptions);
            }
        }

        private void SaveFailedJobs(List<EnterspeedJob> failedJobs)
        {
            var failedJobsToSave = new List<EnterspeedJob>();
            foreach (var failedJob in failedJobs)
            {
                var existingJob = _enterspeedJobRepository.GetFailedJob(failedJob.EntityId);
                if (existingJob != null)
                {
                    existingJob.Exception = failedJob.Exception;
                    existingJob.CreatedAt = failedJob.CreatedAt;
                    _enterspeedJobRepository.Update(existingJob);
                }
                else
                {
                    failedJobsToSave.Add(failedJob);
                }
            }

            // Save all jobs that failed
            _enterspeedJobRepository.Save(failedJobsToSave);
        }

        private void RemoveFromFailedJobs(IReadOnlyCollection<EnterspeedJob> failedJobsToDelete)
        {
            if (failedJobsToDelete.Any())
            {
                _enterspeedJobRepository.Delete(failedJobsToDelete.Select(x => x.Id).Concat(failedJobsToDelete.Select(x => x.Id)).ToList());
            }
        }
    }
}