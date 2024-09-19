using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Enterspeed.Source.UmbracoCms.Base.Services.DataProperties;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.Base.NotificationHandlers
{
    public class EnterspeedContentUnpublishingNotificationHandler
        : BaseEnterspeedNotificationHandler,
        INotificationHandler<ContentUnpublishingNotification>,
            INotificationHandler<ContentMovedToRecycleBinNotification>
    {
        private readonly IContentService _contentService;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;
        private readonly IEnterspeedMasterContentService _enterspeedMasterContentService;

        public EnterspeedContentUnpublishingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IContentService contentService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IUmbracoCultureProvider umbracoCultureProvider,
            IAuditService auditService,
            IServerRoleAccessor serverRoleAccessor,
            IEnterspeedMasterContentService enterspeedMasterContentService,
            ILogger<EnterspeedContentUnpublishingNotificationHandler> logger)
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
            _contentService = contentService;
            _enterspeedJobFactory = enterspeedJobFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
            _enterspeedMasterContentService = enterspeedMasterContentService;
        }

        /// <summary>
        /// Note: The Umbraco unpublishing notification is only fired when entire document is unpublished (all variants)
        /// Unpublishing a culture needs to update the published cache, and therefore triggers the publishing notifications
        /// </summary>
        /// <param name="notification"></param>
        public void Handle(ContentUnpublishingNotification notification)
        {
            var entities = notification.UnpublishedEntities.ToList();
            HandleUnpublishing(entities, false);
        }

        public void Handle(ContentMovedToRecycleBinNotification notification)
        {
            var entities = notification.MoveInfoCollection.Select(x => x.Entity).ToList();
            HandleUnpublishing(entities, true);
        }

        private void HandleUnpublishing(List<IContent> entities, bool unpublishFromPreview)
        {
            var isPublishConfigured = IsPublishConfigured();
            var isPreviewConfigured = IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            if (!entities.Any())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    var cultures = content.ContentType.VariesByCulture()
                        ? _umbracoCultureProvider.GetCulturesForCultureVariant(content)
                        : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(content) };

                    List<IContent> descendants = null;
                    foreach (var culture in cultures)
                    {
                        if (isPublishConfigured)
                        {
                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(content, culture, EnterspeedContentState.Publish));
                        }

                        if (isPreviewConfigured && unpublishFromPreview)
                        {
                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(content, culture, EnterspeedContentState.Preview));
                        }

                        if (descendants == null)
                        {
                            descendants = _contentService.GetPagedDescendants(
                                content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                        }

                        foreach (var descendant in descendants)
                        {
                            var descendantCultures = descendant.ContentType.VariesByCulture()
                                ? _umbracoCultureProvider.GetCulturesForCultureVariant(descendant)
                                : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(descendant) };
                            foreach (var descendantCulture in descendantCultures)
                            {
                                if (isPublishConfigured)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, descendantCulture, EnterspeedContentState.Publish));
                                }

                                if (isPreviewConfigured && unpublishFromPreview)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, descendantCulture, EnterspeedContentState.Preview));
                                }
                            }
                        }
                    }
                }
            }

            if (_enterspeedMasterContentService.IsMasterContentEnabled())
            {
                jobs.AddRange(_enterspeedMasterContentService.CreateDeleteMasterContentJobs(jobs));
            }

            EnqueueJobs(jobs);
        }
    }
}