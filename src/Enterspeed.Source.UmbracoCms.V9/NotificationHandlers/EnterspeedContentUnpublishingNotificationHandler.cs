using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Factories;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.NotificationHandlers
{
    public class EnterspeedContentUnpublishingNotificationHandler
        : BaseEnterspeedNotificationHandler,
        INotificationHandler<ContentUnpublishingNotification>,
            INotificationHandler<ContentMovedToRecycleBinNotification>
    {
        private readonly IContentService _contentService;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedContentUnpublishingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobHandler enterspeedJobHandler,
            IUmbracoContextFactory umbracoContextFactory,
            IContentService contentService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService)
            : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobHandler,
                  umbracoContextFactory,
                  scopeProvider,
                  auditService)
        {
            _contentService = contentService;
            _enterspeedJobFactory = enterspeedJobFactory;
        }

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
                        ? content.AvailableCultures
                        : new List<string> { GetDefaultCulture(context) };

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
                                ? descendant.AvailableCultures
                                : new List<string> { GetDefaultCulture(context) };
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

            EnqueueJobs(jobs);
        }
    }
}