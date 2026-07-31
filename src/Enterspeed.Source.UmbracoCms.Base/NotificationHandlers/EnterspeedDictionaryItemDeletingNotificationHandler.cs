using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Models;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
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
    public class EnterspeedDictionaryItemDeletingNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<DictionaryItemDeletingNotification>
    {
#if NET10_0_OR_GREATER
        private readonly IDictionaryItemService _dictionaryItemService;
#else
        private readonly ILocalizationService _localizationService;
#endif
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IEnterspeedDictionaryTranslation _enterspeedDictionaryTranslation;

        public EnterspeedDictionaryItemDeletingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
#if NET10_0_OR_GREATER
            IDictionaryItemService dictionaryItemService,
#else
            ILocalizationService localizationService,
#endif
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService,
            IServerRoleAccessor serverRoleAccessor,
            ILogger<EnterspeedDictionaryItemDeletingNotificationHandler> logger,
            IEnterspeedDictionaryTranslation enterspeedDictionaryTranslation)
            : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobsHandlingService,
                  umbracoContextFactory,
                  scopeProvider,
                  auditService,
                  serverRoleAccessor,
                  logger)
        {
#if NET10_0_OR_GREATER
            _dictionaryItemService = dictionaryItemService;
#else
            _localizationService = localizationService;
#endif
            _enterspeedJobFactory = enterspeedJobFactory;
            _enterspeedDictionaryTranslation = enterspeedDictionaryTranslation;
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
                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(dictionaryItem, _enterspeedDictionaryTranslation.GetIsoCode(translation), EnterspeedContentState.Publish));
                        }

                        if (isPreviewConfigured)
                        {
                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(dictionaryItem, _enterspeedDictionaryTranslation.GetIsoCode(translation), EnterspeedContentState.Preview));
                        }

                        if (descendants == null)
                        {
#if NET10_0_OR_GREATER
                            descendants = _dictionaryItemService.GetDescendantsAsync(dictionaryItem.Key).GetAwaiter().GetResult().ToList();
#else
                            descendants = _localizationService.GetDictionaryItemDescendants(dictionaryItem.Key).ToList();
#endif
                        }

                        foreach (var descendant in descendants)
                        {
                            foreach (var descendanttranslation in descendant.Translations)
                            {
                                if (isPublishConfigured)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, _enterspeedDictionaryTranslation.GetIsoCode(descendanttranslation), EnterspeedContentState.Publish));
                                }

                                if (isPreviewConfigured)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, _enterspeedDictionaryTranslation.GetIsoCode(descendanttranslation), EnterspeedContentState.Preview));
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
