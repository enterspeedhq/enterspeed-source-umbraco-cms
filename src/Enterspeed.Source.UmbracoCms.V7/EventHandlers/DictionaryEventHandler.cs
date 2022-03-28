using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Enterspeed.Source.UmbracoCms.V7.Factories;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V7.EventHandlers
{
    public class DictionaryEventHandler : ApplicationEventHandler
    {
        private static readonly EnterspeedJobFactory EnterspeedJobFactory = EnterspeedContext.Current.Services.JobFactory;
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            LocalizationService.SavedDictionaryItem += LocalizationServiceOnSavedDictionaryItem;
            LocalizationService.DeletingDictionaryItem += LocalizationServiceOnDeletedDictionaryItem;
        }

        private static void LocalizationServiceOnSavedDictionaryItem(ILocalizationService sender, SaveEventArgs<IDictionaryItem> e)
        {
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();
            var isPreviewConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPreviewConfigured();
            if (!isPublishConfigured && !isPreviewConfigured)
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

                    if (isPublishConfigured)
                    {
                        jobs.Add(EnterspeedJobFactory.GetPublishJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Publish));
                    }

                    if (isPreviewConfigured)
                    {
                        jobs.Add(EnterspeedJobFactory.GetPublishJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Preview));
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        private static void LocalizationServiceOnDeletedDictionaryItem(ILocalizationService sender, DeleteEventArgs<IDictionaryItem> e)
        {
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();
            var isPreviewConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPreviewConfigured();
            if (!isPublishConfigured && !isPreviewConfigured)
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

                    if (isPublishConfigured)
                    {
                        jobs.Add(EnterspeedJobFactory.GetDeleteJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Publish));
                    }

                    if (isPreviewConfigured)
                    {
                        jobs.Add(EnterspeedJobFactory.GetDeleteJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Preview));
                    }

                    if (descendants == null)
                    {
                        descendants = ApplicationContext.Current.Services.LocalizationService.GetDictionaryItemDescendants(dictionaryItem.Key).ToList();
                    }

                    foreach (var descendant in descendants)
                    {
                        foreach (var descendantTranslation in descendant.Translations)
                        {
                            if (isPublishConfigured)
                            {
                                jobs.Add(EnterspeedJobFactory.GetDeleteJob(descendant, descendantTranslation.Language.IsoCode, EnterspeedContentState.Publish));
                            }

                            if (isPreviewConfigured)
                            {
                                jobs.Add(EnterspeedJobFactory.GetDeleteJob(descendant, descendantTranslation.Language.IsoCode, EnterspeedContentState.Preview));
                            }
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
    }
}
