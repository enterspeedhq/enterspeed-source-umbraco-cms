using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Models;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Base.Handlers
{
    public class EnterspeedPostJobsHandler : IEnterspeedPostJobsHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedConfigurationService _configuration;
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly EnterspeedJobHandlerCollection _enterspeedJobHandlerCollection;
        private readonly ILogger<EnterspeedPostJobsHandler> _logger;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;


        public EnterspeedPostJobsHandler(IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobFactory enterspeedJobFactory,
            IEnterspeedConfigurationService configuration,
            ILocalizationService localizationService,
            EnterspeedJobHandlerCollection enterspeedJobHandlerCollection,
            ILogger<EnterspeedPostJobsHandler> logger,
            IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobFactory = enterspeedJobFactory;
            _configuration = configuration;
            _localizationService = localizationService;
            _enterspeedJobHandlerCollection = enterspeedJobHandlerCollection;
            _logger = logger;
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        /// <summary>
        /// Handles cleanup after the initial jobs-handler has done its work
        /// </summary>
        /// <param name="processedJobs"></param>
        /// <param name="existingFailedJobsToDelete"></param>
        /// <param name="newFailedJobs"></param>
        public void Handle(IList<EnterspeedJob> processedJobs,
            IReadOnlyCollection<EnterspeedJob> existingFailedJobsToDelete,
            IList<EnterspeedJob> newFailedJobs)
        {
            if (!_enterspeedConfigurationService.IsRootDictionariesDisabled())
            {
                HandleRootDictionaries(processedJobs);
            }

            HandleProcessedJobs(processedJobs);
            HandleExistingFailedJobs(existingFailedJobsToDelete);

            // WARNING: This throws an exception if any new failed jobs. This should should not be called before any other method
            // TODO: Handle in a different way?
            HandleFailedJobs(newFailedJobs);
        }

        /// <summary>
        /// Removes all processed jobs from the database and thereby effectively removing the jobs from the queue that
        /// has been processed.
        /// </summary>
        /// <param name="processedJobs"></param>
        private void HandleProcessedJobs(IEnumerable<EnterspeedJob> processedJobs)
        {
            _enterspeedJobRepository.Delete(processedJobs.Select(j => j.Id).ToList());
        }

        /// <summary>
        /// Checks if any processed jobs of type dictionary present and creates a virtual root source entity in Enterspeed.
        /// </summary>
        /// <param name="jobs"></param>
        private void HandleRootDictionaries(IEnumerable<EnterspeedJob> jobs)
        {
            // Check if any dictionaries amongst handled jobs that are not a global dictionary.
            // If any then create root dictionary items
            if (!jobs.Any(j =>
                    j.EntityType == EnterspeedJobEntityType.Dictionary &&
                    !j.EntityId.Equals(UmbracoDictionariesRootEntity.EntityId))) return;

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

                foreach (var rootJob in dictionaryItemsRootJobs)
                {
                    var handler = _enterspeedJobHandlerCollection.FirstOrDefault(jh => jh.CanHandle(rootJob));
                    if (handler == null)
                    {
                        var message = $"No job handler available for {rootJob.EntityId} {rootJob.EntityType}";
                        _logger.LogWarning(message);
                        continue;
                    }

                    try
                    {
                        handler.Handle(rootJob);
                    }
                    catch (Exception exception)
                    {
                        // Exceptions has a ToString() override which formats the full exception nicely.
                        var exceptionAsString = exception.ToString();
                        _logger.LogError(exceptionAsString);
                    }
                }
            }
        }

        /// <summary>
        /// Jobs that failed while processing are being created or updated in the database.
        /// Update is happening to avoid multiple database entries of the same error.
        /// </summary>
        /// <param name="newFailedJobs"></param>
        /// <exception cref="Exception"></exception>
        public void HandleFailedJobs(IList<EnterspeedJob> newFailedJobs)
        {
            if (!newFailedJobs.Any()) return;
            var failedJobsToSave = new List<EnterspeedJob>();
            foreach (var failedJob in newFailedJobs)
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
            var failedJobExceptions = string.Join(Environment.NewLine, newFailedJobs.Select(x => x.Exception));
            throw new Exception(failedJobExceptions);
        }

        /// <summary>
        /// Removes existing failed jobs that has successfully been processed.
        /// </summary>
        /// <param name="existingFailedJobs"></param>
        private void HandleExistingFailedJobs(IReadOnlyCollection<EnterspeedJob> existingFailedJobs)
        {
            if (existingFailedJobs.Any())
            {
                _enterspeedJobRepository.Delete(existingFailedJobs.Select(x => x.Id)
                    .Concat(existingFailedJobs.Select(x => x.Id)).ToList());
            }
        }
    }
}