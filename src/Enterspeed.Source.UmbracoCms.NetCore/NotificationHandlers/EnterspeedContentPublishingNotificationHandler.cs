using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.NetCore.Data.Models;
using Enterspeed.Source.UmbracoCms.NetCore.Data.Repositories;
using Enterspeed.Source.UmbracoCms.NetCore.Factories;
using Enterspeed.Source.UmbracoCms.NetCore.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
#if U9
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.NetCore.NotificationHandlers
{
    public class EnterspeedContentPublishingNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<ContentPublishingNotification>
    {
        private readonly IContentService _contentService;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedContentPublishingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IContentService contentService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService)
            : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobsHandlingService,
                  umbracoContextFactory,
                  scopeProvider,
                  auditService)
        {
            _contentService = contentService;
            _enterspeedJobFactory = enterspeedJobFactory;
        }

        public void Handle(ContentPublishingNotification notification)
        {
            ContentServicePublishing(notification.PublishedEntities.ToList());
        }

        private void ContentServicePublishing(List<IContent> entities)
        {
            var isPublishConfigured = IsPublishConfigured();
            var isPreviewConfigured = IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            // This only handles variants that has been unpublished. Publishing is handled in the ContentCacheUpdated method
            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    if (!content.ContentType.VariesByCulture())
                    {
                        continue;
                    }

                    List<IContent> descendants = null;

                    foreach (var culture in content.AvailableCultures)
                    {
                        var isCultureUnpublished = content.IsPropertyDirty(ContentBase.ChangeTrackingPrefix.UnpublishedCulture + culture);
                        if (isCultureUnpublished)
                        {
                            if (isPublishConfigured)
                            {
                                jobs.Add(_enterspeedJobFactory.GetDeleteJob(content, culture, EnterspeedContentState.Publish));
                            }

                            if (isPreviewConfigured)
                            {
                                jobs.Add(_enterspeedJobFactory.GetDeleteJob(content, culture, EnterspeedContentState.Preview));
                            }

                            if (descendants == null)
                            {
                                descendants = _contentService
                                    .GetPagedDescendants(content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                            }

                            foreach (var descendant in descendants)
                            {
                                if (descendant.ContentType.VariesByCulture())
                                {
                                    var descendantCultures = descendant.AvailableCultures;
                                    if (descendantCultures.Contains(culture))
                                    {
                                        if (isPublishConfigured)
                                        {
                                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, culture, EnterspeedContentState.Publish));
                                        }

                                        if (isPreviewConfigured)
                                        {
                                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, culture, EnterspeedContentState.Preview));
                                        }
                                    }
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