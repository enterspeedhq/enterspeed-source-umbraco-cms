using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Models;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Web;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.Base.NotificationHandlers
{
    public class EnterspeedDictionaryItemSavedNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<DictionaryItemSavedNotification>
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IEnterspeedDictionaryTranslation _enterspeedDictionaryTranslation;

        public EnterspeedDictionaryItemSavedNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IEnterspeedDictionaryTranslation enterspeedDictionaryTranslation,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService,
            IServerRoleAccessor serverRoleAccessor,
            ILogger<EnterspeedDictionaryItemSavedNotificationHandler> logger) : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobsHandlingService,
                  umbracoContextFactory,
                  scopeProvider,
                  auditService,
                  serverRoleAccessor,
                  logger)
        {
            _enterspeedDictionaryTranslation = enterspeedDictionaryTranslation;
            _enterspeedJobFactory = enterspeedJobFactory;
        }

        public void Handle(DictionaryItemSavedNotification notification)
        {
            var isPublishConfigured = IsPublishConfigured();
            var isPreviewConfigured = IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            var entities = notification.SavedEntities.ToList();
            var jobs = new List<EnterspeedJob>();
            foreach (var dictionaryItem in entities)
            {
                foreach (var translation in dictionaryItem.Translations)
                {
                    if (isPublishConfigured)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, _enterspeedDictionaryTranslation.GetIsoCode(translation), EnterspeedContentState.Publish));
                    }

                    if (isPreviewConfigured)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, _enterspeedDictionaryTranslation.GetIsoCode(translation), EnterspeedContentState.Preview));
                    }
                }
            }

            EnqueueJobs(jobs);
        }
    }
}
