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
    public class EnterspeedContentUnpublishingNotificationHandler
        : BaseEnterspeedNotificationHandler,
        INotificationHandler<ContentUnpublishingNotification>,
            INotificationHandler<ContentMovedToRecycleBinNotification>
    {
        private readonly IContentService _contentService;

        public EnterspeedContentUnpublishingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobHandler enterspeedJobHandler,
            IUmbracoContextFactory umbracoContextFactory,
            IContentService contentService,
            IScopeProvider scopeProvider) : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobHandler,
                  umbracoContextFactory,
                  scopeProvider)
        {
            _contentService = contentService;
        }

        public void Handle(ContentUnpublishingNotification notification)
        {
            var entities = notification.UnpublishedEntities.ToList();
            HandleUnpublishing(entities);
        }

        public void Handle(ContentMovedToRecycleBinNotification notification)
        {
            var entities = notification.MoveInfoCollection.Select(x => x.Entity).ToList();
            HandleUnpublishing(entities);
        }

        private void HandleUnpublishing(List<IContent> entities)
        {
            if (!IsConfigured())
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
                                jobs.Add(
                                    new EnterspeedJob
                                    {
                                        EntityId = descendant.Id.ToString(),
                                        EntityType = EnterspeedJobEntityType.Content,
                                        Culture = descendantCulture,
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