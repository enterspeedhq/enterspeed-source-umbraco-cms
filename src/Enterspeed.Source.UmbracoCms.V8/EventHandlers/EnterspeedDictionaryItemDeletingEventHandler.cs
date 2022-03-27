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
    public class EnterspeedDictionaryItemDeletingEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        public EnterspeedDictionaryItemDeletingEventHandler(
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
            LocalizationService.DeletingDictionaryItem += LocalizationServiceOnDeletedDictionaryItem;
        }

        public void LocalizationServiceOnDeletedDictionaryItem(
            ILocalizationService sender, DeleteEventArgs<IDictionaryItem> e)
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
                        jobs.Add(
                            new EnterspeedJob
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
                            descendants = Current.Services.LocalizationService
                                .GetDictionaryItemDescendants(dictionaryItem.Key).ToList();
                        }

                        foreach (var descendant in descendants)
                        {
                            foreach (var descendanttranslation in descendant.Translations)
                            {
                                jobs.Add(
                                    new EnterspeedJob
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


        public void Terminate()
        {
        }
    }
}