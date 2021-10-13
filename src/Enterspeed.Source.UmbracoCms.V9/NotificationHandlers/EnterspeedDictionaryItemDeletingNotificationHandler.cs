using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.V9.NotificationHandlers
{
    public class EnterspeedDictionaryItemDeletingNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<DictionaryItemDeletingNotification>
    {
        private readonly ILocalizationService _localizationService;

        public EnterspeedDictionaryItemDeletingNotificationHandler(
            IEnterspeedConfigurationService configurationService, 
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobHandler enterspeedJobHandler,
            IUmbracoContextFactory umbracoContextFactory,
            ILocalizationService localizationService,
            IScopeProvider scopeProvider) : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobHandler,
                  umbracoContextFactory,
                  scopeProvider)
        {
            _localizationService = localizationService;
        }

        public void Handle(DictionaryItemDeletingNotification notification)
        {
            if (!IsConfigured())
            {
                return;
            }

            var entities = notification.DeletedEntities.ToList();
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
                            descendants = _localizationService.GetDictionaryItemDescendants(dictionaryItem.Key).ToList();
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
    }
}
