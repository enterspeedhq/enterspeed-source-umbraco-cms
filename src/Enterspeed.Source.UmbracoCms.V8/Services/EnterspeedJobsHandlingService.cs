using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Handlers;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Sync;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class EnterspeedJobsHandlingService : IEnterspeedJobsHandlingService
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedJobsHandler _enterspeedJobsHandler;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;
        private readonly IRuntimeState _runtimeState;
        private readonly ILogger _logger;

        public EnterspeedJobsHandlingService(
            IEnterspeedJobRepository enterspeedJobRepository,
            ILogger logger,
            IEnterspeedJobsHandler enterspeedJobsHandler,
            IEnterspeedConfigurationService enterspeedConfigurationService,
            IRuntimeState runtimeState)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _logger = logger;
            _enterspeedJobsHandler = enterspeedJobsHandler;
            _enterspeedConfigurationService = enterspeedConfigurationService;
            _runtimeState = runtimeState;
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
                    _logger.Error<EnterspeedJobsHandlingService>(e, "Error has happened");
                }
            }
            while (jobCount > 0);
        }

        public bool IsJobsProcessingEnabled()
        {
            var configuration = _enterspeedConfigurationService.GetConfiguration();
            return configuration.RunJobsOnAllServerRoles
                   || _runtimeState.ServerRole == ServerRole.Master
                   || _runtimeState.ServerRole == ServerRole.Single;
        }

        public virtual void HandleJobs(IList<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            _logger.Debug<EnterspeedJobsHandlingService>("Handling {jobsCount} jobs", jobs.Count);

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
    }
}