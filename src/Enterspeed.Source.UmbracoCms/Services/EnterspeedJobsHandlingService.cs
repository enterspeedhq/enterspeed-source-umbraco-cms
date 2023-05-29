using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Handlers;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public class EnterspeedJobsHandlingService : IEnterspeedJobsHandlingService
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedJobsHandler _enterspeedJobsHandler;
        private readonly ILogger<EnterspeedJobsHandlingService> _logger;

        public EnterspeedJobsHandlingService(
            IEnterspeedJobRepository enterspeedJobRepository,
            ILogger<EnterspeedJobsHandlingService> logger,
            IEnterspeedJobsHandler enterspeedJobsHandler)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _logger = logger;
            _enterspeedJobsHandler = enterspeedJobsHandler;
        }

        public virtual void HandlePendingJobs(int batchSize)
        {
            int jobCount;
            do
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
            } while (jobCount > 0);
        }

        public virtual void HandleJobs(IList<EnterspeedJob> jobs)
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
            _enterspeedJobsHandler.HandleJobs(jobs);
        }

        public virtual void InvalidateOldProcessingJobs()
        {
            var oldJobs = _enterspeedJobRepository.GetOldProcessingTasks().ToList();
            if (!oldJobs.Any()) return;

            // Updating old processing jobs, so they are set as failed instead. 
            foreach (var job in oldJobs)
            {
                job.State = EnterspeedJobState.Failed;
                job.Exception = $"Job processing timed out. Last updated at: {job.UpdatedAt}";
                job.UpdatedAt = DateTime.UtcNow;
            }

            // Save new failed job or update existing failed job.
            _enterspeedJobsHandler.SaveOrUpdateFailedJobs(oldJobs);
        }
    }
}