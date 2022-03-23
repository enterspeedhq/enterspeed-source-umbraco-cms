using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.V9.NotificationHandlers
{
    public class EnterspeedDictionaryItemSavedNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<DictionaryItemSavedNotification>
    {
        public EnterspeedDictionaryItemSavedNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider) : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobsHandlingService,
                  umbracoContextFactory,
                  scopeProvider)
        {
        }

        public void Handle(DictionaryItemSavedNotification notification)
        {
            if (!IsConfigured())
            {
                return;
            }

            var entities = notification.SavedEntities.ToList();
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
    }
}
