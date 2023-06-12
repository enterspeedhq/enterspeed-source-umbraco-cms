using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Factories;
using Enterspeed.Source.UmbracoCms.Services;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Handlers
{
    public class EnterspeedPostJobsHandler : IEnterspeedPostJobsHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedConfigurationService _configuration;
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedPostJobsHandler(IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobFactory enterspeedJobFactory,
            IEnterspeedConfigurationService configuration,
            ILocalizationService localizationService)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobFactory = enterspeedJobFactory;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Handles cleanup after the initial jobs-handler has done its work
        /// </summary>
        /// <param name="processedJobs"></param>
        /// <param name="existingFailedJobs"></param>
        /// <param name="newFailedJobs"></param>
        public void Handle(IList<EnterspeedJob> processedJobs,
            IReadOnlyCollection<EnterspeedJob> existingFailedJobs,
            IList<EnterspeedJob> newFailedJobs)
        {
            HandleRootDictionaries(processedJobs);
            RemoveProcessedJobs(processedJobs);
            RemoveExistingFailedJobs(existingFailedJobs);

            // WARNING: This throws an exception if any new failed jobs. This should should not be called before any other method
            // TODO: Handle in a different way?
            UpsertFailedJobs(newFailedJobs);
        }

        /// <summary>
        /// Removes all processed jobs from the database and thereby effectively removing the jobs that has been processed
        /// from the queue.
        /// </summary>
        /// <param name="processedJobs"></param>
        private void RemoveProcessedJobs(IEnumerable<EnterspeedJob> processedJobs)
        {
            _enterspeedJobRepository.Delete(processedJobs.Select(j => j.Id).ToList());
        }

        /// <summary>
        /// Checks of any processed jobs of type dictionary present and creates a virtual root source entity in enterspeed.
        /// </summary>
        /// <param name="jobs"></param>
        private void HandleRootDictionaries(IEnumerable<EnterspeedJob> jobs)
        {
            // Check if any dictionaries amongst handled jobs. If any then create root dictionary items
            if (jobs.Any(a => a.EntityType == EnterspeedJobEntityType.Dictionary))
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
                        .Select(isoCode => _enterspeedJobFactory.GetDictionaryItemRootJob(isoCode, destination.Key))
                        .ToList();

                    // Add to queue. We dont want to make recursive calls directly to the jobs handler
                    _enterspeedJobRepository.Save(dictionaryItemsRootJobs);
                }
            }
        }

        /// <summary>
        /// Jobs that failed while processing are being created or updated in the database.
        /// Update is happening to avoid multiple database entries of the same error.
        /// </summary>
        /// <param name="failedJobs"></param>
        /// <exception cref="Exception"></exception>
        public void UpsertFailedJobs(IList<EnterspeedJob> failedJobs)
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

            _enterspeedJobRepository.Save(failedJobsToSave);

            // Throw exception with a combined exception message for all jobs that failed if any
            var failedJobExceptions = string.Join(Environment.NewLine, failedJobs.Select(x => x.Exception));
            throw new Exception(failedJobExceptions);
        }

        /// <summary>
        /// Removes existing failed jobs that has successfully been processed
        /// </summary>
        /// <param name="failedJobsToDelete"></param>
        private void RemoveExistingFailedJobs(IReadOnlyCollection<EnterspeedJob> failedJobsToDelete)
        {
            if (failedJobsToDelete.Any())
            {
                _enterspeedJobRepository.Delete(failedJobsToDelete.Select(x => x.Id)
                    .Concat(failedJobsToDelete.Select(x => x.Id)).ToList());
            }
        }
    }
}