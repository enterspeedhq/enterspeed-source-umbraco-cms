using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Factories;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.EventHandlers
{
    public class EnterspeedDictionaryItemDeletingEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedDictionaryItemDeletingEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IRuntimeState runtime,
            ILogger logger)
            : base(
                umbracoContextFactory,
                enterspeedJobRepository,
                jobsHandlingService,
                configurationService,
                scopeProvider,
                runtime, 
                logger)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
        }

        public void Initialize()
        {
            LocalizationService.DeletingDictionaryItem += LocalizationServiceOnDeletedDictionaryItem;
        }

        public void LocalizationServiceOnDeletedDictionaryItem(
            ILocalizationService sender, DeleteEventArgs<IDictionaryItem> e)
        {
            var isPublishConfigured = ConfigurationService.IsPublishConfigured();
            var isPreviewConfigured = ConfigurationService.IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            var entities = e.DeletedEntities.ToList();
            var jobs = new List<EnterspeedJob>();
            using (var context = UmbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var dictionaryItem in entities)
                {
                    List<IDictionaryItem> descendants = null;
                    foreach (var translation in dictionaryItem.Translations)
                    {
                        if (isPublishConfigured)
                        {
                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Publish));
                        }

                        if (isPreviewConfigured)
                        {
                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Preview));
                        }

                        if (descendants == null)
                        {
                            descendants = Current.Services.LocalizationService.GetDictionaryItemDescendants(dictionaryItem.Key).ToList();
                        }

                        foreach (var descendant in descendants)
                        {
                            foreach (var descendanttranslation in descendant.Translations)
                            {
                                if (isPublishConfigured)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, descendanttranslation.Language.IsoCode, EnterspeedContentState.Publish));
                                }

                                if (isPreviewConfigured)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, descendanttranslation.Language.IsoCode, EnterspeedContentState.Preview));
                                }
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