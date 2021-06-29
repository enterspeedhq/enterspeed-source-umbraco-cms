using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Handlers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Components
{
    public class EnterspeedDictionaryEventsComponent : IComponent
    {
        private readonly IEnterspeedConfigurationService _configurationService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedJobHandler _enterspeedJobHandler;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IScopeProvider _scopeProvider;

        public EnterspeedDictionaryEventsComponent(
            IEnterspeedConfigurationService configurationService,
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobHandler enterspeedJobHandler,
            IEnterspeedJobRepository enterspeedJobRepository,
            IScopeProvider scopeProvider)
        {
            _configurationService = configurationService;
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedJobHandler = enterspeedJobHandler;
            _enterspeedJobRepository = enterspeedJobRepository;
            _scopeProvider = scopeProvider;
        }

        public void Initialize()
        {
            LocalizationService.SavedDictionaryItem += LocalizationServiceOnSavedDictionaryItem;
            LocalizationService.DeletingDictionaryItem += LocalizationServiceOnDeletedDictionaryItem;
        }

        public void Terminate()
        {
        }

        private void LocalizationServiceOnSavedDictionaryItem(ILocalizationService sender, SaveEventArgs<IDictionaryItem> e)
        {
            if (!IsConfigured())
            {
                return;
            }

            var entities = e.SavedEntities.ToList();
            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var dictionaryItem in entities)
                {
                    foreach (var translation in dictionaryItem.Translations)
                    {
                        var now = DateTime.UtcNow;
                        jobs.Add(new EnterspeedJob
                        {
                            EntityId = dictionaryItem.Key.ToString(),
                            EntityType = EnterspeedJobEntityType.Dictionary,
                            Culture = translation.Language.IsoCode,
                            JobType = EnterspeedJobType.Publish,
                            State = EnterspeedJobState.Pending,
                            CreatedAt = now,
                            UpdatedAt = now,
                        });
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        private void LocalizationServiceOnDeletedDictionaryItem(ILocalizationService sender, DeleteEventArgs<IDictionaryItem> e)
        {
            if (!IsConfigured())
            {
                return;
            }

            var entities = e.DeletedEntities.ToList();
            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var dictionaryItem in entities)
                {
                    List<IDictionaryItem> descendants = null;
                    foreach (var translation in dictionaryItem.Translations)
                    {
                        var now = DateTime.UtcNow;
                        jobs.Add(new EnterspeedJob
                        {
                            EntityId = dictionaryItem.Key.ToString(),
                            EntityType = EnterspeedJobEntityType.Dictionary,
                            Culture = translation.Language.IsoCode,
                            JobType = EnterspeedJobType.Delete,
                            State = EnterspeedJobState.Pending,
                            CreatedAt = now,
                            UpdatedAt = now,
                        });

                        if (descendants == null)
                        {
                            descendants = Current.Services.LocalizationService.GetDictionaryItemDescendants(dictionaryItem.Key).ToList();
                        }

                        foreach (var descendant in descendants)
                        {
                            foreach (var descendanttranslation in descendant.Translations)
                            {
                                jobs.Add(new EnterspeedJob
                                {
                                    EntityId = descendant.Key.ToString(),
                                    EntityType = EnterspeedJobEntityType.Dictionary,
                                    Culture = descendanttranslation.Language.IsoCode,
                                    JobType = EnterspeedJobType.Delete,
                                    State = EnterspeedJobState.Pending,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                });
                            }
                        }
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        private void EnqueueJobs(List<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            _enterspeedJobRepository.Save(jobs);

            using (_umbracoContextFactory.EnsureUmbracoContext())
            {
                if (_scopeProvider.Context != null)
                {
                    var key = $"UpdateEnterspeed_{DateTime.Now.Ticks}";
                    // Add a callback to the current Scope which will execute when it's completed
                    _scopeProvider.Context.Enlist(key, scopeCompleted => HandleJobs(scopeCompleted, jobs));
                }
            }
        }

        private void HandleJobs(bool scopeCompleted, List<EnterspeedJob> jobs)
        {
            // Do not continue if the scope did not complete - the transaction may have been canceled and rolled back
            if (scopeCompleted)
            {
                _enterspeedJobHandler.HandleJobs(jobs);
            }
        }

        private bool IsConfigured()
        {
            return _configurationService.GetConfiguration().IsConfigured;
        }
    }
}
