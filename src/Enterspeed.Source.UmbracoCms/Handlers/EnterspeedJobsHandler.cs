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
            var newFailedJobs = new List<EnterspeedJob>();

            // Fetch all failed jobs for these content ids. We need to do this to delete the failed jobs if they no longer fails
            var failedJobsToHandle = _enterspeedJobRepository.GetFailedJobs(jobs.Select(x => x.EntityId).Distinct().ToList());
            var existingFailedJobsToDelete = new List<EnterspeedJob>();

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
                    newFailedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestJob, message));
                    _logger.LogWarning(message);
                    continue;
                }

                try
                {
                    handler.Handle(newestJob);

                    // If job has been successfully handled and a failed job exists with same id, we should remove it
                    var existingFailedJobToDelete = failedJobsToHandle.Where(x =>
                        x.EntityId == newestJob.EntityId && x.Culture == newestJob.Culture && x.ContentState == newestJob.ContentState).ToList();

                    existingFailedJobsToDelete.AddRange(existingFailedJobToDelete);
                }
                catch (Exception exception)
                {
                    // Exceptions has a ToString() override which formats the full exception nicely.
                    var exceptionAsString = exception.ToString();
                    newFailedJobs.Add(_enterspeedJobFactory.GetFailedJob(newestJob, exceptionAsString));
                    _logger.LogError(exceptionAsString);
                }
            }

            // Remove existing failed jobs that has been handled
            RemoveExistingFailedJobs(existingFailedJobsToDelete);

            // Save or update new failed jobs to database
            SaveOrUpdateFailedJobs(newFailedJobs);
        }

        public void SaveOrUpdateFailedJobs(List<EnterspeedJob> failedJobs)
        {
            if (!failedJobs.Any()) return;

            var failedJobsToSave = new List<EnterspeedJob>();
            foreach (var failedJob in failedJobs)
            {
                var existingJob = _enterspeedJobRepository.GetFailedJob(failedJob.EntityId);
                if (existingJob != null)
                {
                    existingJob.Exception = failedJob.Exception;
                    existingJob.CreatedAt = failedJob.CreatedAt;
                    existingJob.UpdatedAt = failedJob.CreatedAt;
                    _enterspeedJobRepository.Update(existingJob);
                }
                else
                {
                    failedJobsToSave.Add(failedJob);
                }
            }

            // Save all jobs that failed
            _enterspeedJobRepository.Save(failedJobsToSave);

            // Throw exception with a combined exception message for all jobs that failed if any
            var failedJobExceptions = string.Join(Environment.NewLine, failedJobs.Select(x => x.Exception));
            throw new Exception(failedJobExceptions);
        }

        private void RemoveExistingFailedJobs(IReadOnlyCollection<EnterspeedJob> failedJobsToDelete)
        {
            if (failedJobsToDelete.Any())
            {
                _enterspeedJobRepository.Delete(failedJobsToDelete.Select(x => x.Id).Concat(failedJobsToDelete.Select(x => x.Id)).ToList());
            }
        }
    }
}