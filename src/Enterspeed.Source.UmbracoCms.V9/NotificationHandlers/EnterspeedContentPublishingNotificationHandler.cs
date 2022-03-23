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
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.NotificationHandlers
{
    public class EnterspeedContentPublishingNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<ContentPublishingNotification>
    {
        private readonly IContentService _contentService;

        public EnterspeedContentPublishingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IContentService contentService,
            IScopeProvider scopeProvider)
            : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobsHandlingService,
                  umbracoContextFactory,
                  scopeProvider)
        {
            _contentService = contentService;
        }

        public void Handle(ContentPublishingNotification notification)
        {
            ContentServicePublishing(notification.PublishedEntities.ToList());
        }

        private void ContentServicePublishing(List<IContent> entities)
        {
            if (!IsConfigured())
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
                            var now = DateTime.UtcNow;
                            jobs.Add(
                                new EnterspeedJob
                                {
                                    EntityId = content.Id.ToString(),
                                    EntityType = EnterspeedJobEntityType.Content,
                                    Culture = culture,
                                    JobType = EnterspeedJobType.Delete,
                                    State = EnterspeedJobState.Pending,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                });

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
                                        jobs.Add(
                                            new EnterspeedJob
                                            {
                                                EntityId = descendant.Id.ToString(),
                                                EntityType = EnterspeedJobEntityType.Content,
                                                Culture = culture,
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
                }
            }

            EnqueueJobs(jobs);
        }
    }
}