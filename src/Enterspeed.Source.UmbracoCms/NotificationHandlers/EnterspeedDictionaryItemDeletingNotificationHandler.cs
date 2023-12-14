﻿using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Factories;
using Enterspeed.Source.UmbracoCms.Services;
using Enterspeed.Source.UmbracoCms.Services.DataProperties;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Sync;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.NotificationHandlers
{
    public class EnterspeedDictionaryItemDeletingNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<DictionaryItemDeletingNotification>
    {
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedDictionaryItemDeletingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            ILocalizationService localizationService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService,
            IServerRoleAccessor serverRoleAccessor,
            IEnterspeedMasterContentService enterspeedMasterContentService,
            ILogger<EnterspeedDictionaryItemDeletingNotificationHandler> logger)
            : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobsHandlingService,
                  umbracoContextFactory,
                  scopeProvider,
                  auditService,
                  serverRoleAccessor,
                  enterspeedMasterContentService,
                  logger)
        {
            _localizationService = localizationService;
            _enterspeedJobFactory = enterspeedJobFactory;
        }

        public void Handle(DictionaryItemDeletingNotification notification)
        {
            var isPublishConfigured = _configurationService.IsPublishConfigured();
            var isPreviewConfigured = _configurationService.IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
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
                            descendants = _localizationService.GetDictionaryItemDescendants(dictionaryItem.Key).ToList();
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
    }
}
