using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Handlers;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Scoping;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public class EnterspeedJobsHandlingService : IEnterspeedJobsHandlingService
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedJobsHandler _enterspeedJobsHandler;
        private readonly IEnterspeedPostJobsHandler _enterspeedPostJobsHandler;
        private readonly ILogger<EnterspeedJobsHandlingService> _logger;
        private readonly IScopeProvider _scopeProvider;

        public EnterspeedJobsHandlingService(
            IEnterspeedJobRepository enterspeedJobRepository,
            ILogger<EnterspeedJobsHandlingService> logger,
            IEnterspeedJobsHandler enterspeedJobsHandler,
            IEnterspeedPostJobsHandler enterspeedPostJobsHandler,
            IScopeProvider scopeProvider)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _logger = logger;
            _enterspeedJobsHandler = enterspeedJobsHandler;
            _enterspeedPostJobsHandler = enterspeedPostJobsHandler;
            _scopeProvider = scopeProvider;
        }

        public virtual void HandlePendingJobs(int batchSize)
        {
            int jobCount;
            do
            {
                List<EnterspeedJob> jobs;
                using (_scopeProvider.CreateScope(autoComplete: true))
                {
                    jobs = _enterspeedJobRepository.GetPendingJobs(batchSize).ToList();
                    jobCount = jobs.Count;
                }

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

            using (var scope = _scopeProvider.CreateScope())
            {
                _enterspeedJobRepository.Save(jobs);
                scope.Complete();
            }

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
                job.Exception = $"Job processing timed out for {job.EntityId}. Last updated at: {job.UpdatedAt}";
                job.UpdatedAt = DateTime.UtcNow;
            }

            // Save new failed job or update existing failed job.
            _enterspeedPostJobsHandler.HandleFailedJobs(oldJobs);
        }
    }
}