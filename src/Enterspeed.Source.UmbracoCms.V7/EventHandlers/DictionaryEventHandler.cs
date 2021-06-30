using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V7.EventHandlers
{
    public class DictionaryEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            LocalizationService.SavedDictionaryItem += LocalizationServiceOnSavedDictionaryItem;
            LocalizationService.DeletingDictionaryItem += LocalizationServiceOnDeletedDictionaryItem;
        }

        private static void LocalizationServiceOnSavedDictionaryItem(ILocalizationService sender, SaveEventArgs<IDictionaryItem> e)
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

            EnqueueJobs(jobs);
        }

        private static void LocalizationServiceOnDeletedDictionaryItem(ILocalizationService sender, DeleteEventArgs<IDictionaryItem> e)
        {
            if (!IsConfigured())
            {
                return;
            }

            var entities = e.DeletedEntities.ToList();
            var jobs = new List<EnterspeedJob>();
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
                        descendants = ApplicationContext.Current.Services.LocalizationService.GetDictionaryItemDescendants(dictionaryItem.Key).ToList();
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

            EnqueueJobs(jobs);
        }

        private static void EnqueueJobs(List<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            EnterspeedContext.Current.Repositories.JobRepository.Save(jobs);
            EnterspeedContext.Current.Handlers.JobHandler.HandleJobs(jobs);
        }

        private static bool IsConfigured()
        {
            return EnterspeedContext.Current?.Configuration != null
                   && EnterspeedContext.Current.Configuration.IsConfigured;
        }
    }
}
