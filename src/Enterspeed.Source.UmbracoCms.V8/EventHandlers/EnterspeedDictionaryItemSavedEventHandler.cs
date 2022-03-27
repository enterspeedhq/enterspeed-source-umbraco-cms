using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.EventHandlers
{
    public class EnterspeedDictionaryItemSavedEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        public EnterspeedDictionaryItemSavedEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider)
            : base(
                umbracoContextFactory,
                enterspeedJobRepository,
                jobsHandlingService,
                configurationService,
                scopeProvider)
        {
        }

        public void Initialize()
        {
            LocalizationService.SavedDictionaryItem += LocalizationServiceOnSavedDictionaryItem;
        }

        public void LocalizationServiceOnSavedDictionaryItem(
            ILocalizationService sender, SaveEventArgs<IDictionaryItem> e)
        {
            if (!IsConfigured())
            {
                return;
            }

            var entities = e.SavedEntities.ToList();
            var jobs = new List<EnterspeedJob>();
            foreach (var dictionaryItem in entities)
            {
                foreach (var translation in dictionaryItem.Translations)
                {
                    var now = DateTime.UtcNow;
                    jobs.Add(
                        new EnterspeedJob
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

            EnqueueJobs(jobs);
        }

        public void Terminate()
        {
        }
    }
}