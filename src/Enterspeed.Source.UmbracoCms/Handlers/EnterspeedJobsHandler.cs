using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Factories;
using Enterspeed.Source.UmbracoCms.Models;
using Enterspeed.Source.UmbracoCms.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Handlers
{
    public class EnterspeedJobsHandler : IEnterspeedJobsHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly ILogger<EnterspeedJobsHandler> _logger;
        private readonly EnterspeedJobHandlerCollection _jobHandlers;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IEnterspeedConfigurationService _configuration;
        private readonly ILocalizationService _localizationService;

        public EnterspeedJobsHandler(
            IEnterspeedJobRepository enterspeedJobRepository,
            ILogger<EnterspeedJobsHandler> logger,
            EnterspeedJobHandlerCollection jobHandlers,
            IEnterspeedJobFactory enterspeedJobFactory,
            IEnterspeedConfigurationService configuration,
            ILocalizationService localizationService)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _logger = logger;
            _jobHandlers = jobHandlers;
            _enterspeedJobFactory = enterspeedJobFactory;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        public void HandleJobs(IList<EnterspeedJob> jobs)
        {
            // Process nodes
            var newFailedJobs = new List<EnterspeedJob>();

            // Fetch all failed jobs for these content ids. We need to do this to delete the failed jobs if they no longer fails
            var failedJobsToHandle =
                _enterspeedJobRepository.GetFailedJobs(jobs.Select(x => x.EntityId).Distinct().ToList());
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
                        x.EntityId == newestJob.EntityId && x.Culture == newestJob.Culture &&
                        x.ContentState == newestJob.ContentState).ToList();

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

            // Check if any dictionaries amongst handled jobs. If any then create root dictionary items
            if (jobs.Any(a => a.EntityType == EnterspeedJobEntityType.Dictionary))
            {
                HandleRootDictionaries();
            }

                // Remove existing failed jobs that has been handled
                RemoveExistingFailedJobs(existingFailedJobsToDelete);

                // Delete all jobs queued for processing
                _enterspeedJobRepository.Delete(jobs.Select(j => j.Id).ToList());

                // Save or update new failed jobs to database
                SaveOrUpdateFailedJobs(newFailedJobs);
            }

        private void HandleRootDictionaries()
        {
            var languageIsoCodes = _localizationService.GetAllLanguages()
                .Select(s => s.IsoCode)
                .ToList();

            // Per configured destination, process separate jobs
            var stateConfigurations = new Dictionary<EnterspeedContentState, bool>()
            {
                { EnterspeedContentState.Preview, _configuration.IsPreviewConfigured() },
                { EnterspeedContentState.Publish, _configuration.IsPublishConfigured() },
            };

            foreach (var destination in stateConfigurations.Where(w => w.Value))
            {
                // Create job per culture of requested dictionary items
                var dictionaryItemsRootJobs = languageIsoCodes
                    .Select(isoCode => GetDictionaryItemsRootJob(isoCode, destination.Key))
                    .ToList();

                // Has to be handled in the end, when dictionary items are ingested
                HandleJobs(dictionaryItemsRootJobs);
            }
        }

        public void SaveOrUpdateFailedJobs(List<EnterspeedJob> failedJobs)
        {
            if (!failedJobs.Any()) return;
            var failedJobsToSave = new List<EnterspeedJob>();
            foreach (var failedJob in failedJobs)
            {
                var existingJob = _enterspeedJobRepository.GetFailedJob(failedJob.EntityId, failedJob.Culture);
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
                _enterspeedJobRepository.Delete(failedJobsToDelete.Select(x => x.Id)
                    .Concat(failedJobsToDelete.Select(x => x.Id)).ToList());
            }
        }

        protected EnterspeedJob GetDictionaryItemsRootJob(string culture, EnterspeedContentState contentState)
        {
            return new EnterspeedJob
            {
                EntityId = UmbracoDictionariesRootEntity.EntityId,
                EntityType = EnterspeedJobEntityType.Dictionary,
                Culture = culture,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                JobType = EnterspeedJobType.Publish,
                State = EnterspeedJobState.Processing,
                ContentState = contentState
            };
        }
    }
}