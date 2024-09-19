using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
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
    public class EnterspeedContentPublishingNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<ContentPublishingNotification>
    {
        private readonly IContentService _contentService;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;

        public EnterspeedContentPublishingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IContentService contentService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService,
            IUmbracoCultureProvider umbracoCultureProvider,
            IServerRoleAccessor serverRoleAccessor,
            ILogger<EnterspeedContentPublishingNotificationHandler> logger)
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

            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    HandleUnpublishedVariants(content, isPublishConfigured, jobs, isPreviewConfigured);
                    HandleParentNameChange(context, content, isPublishConfigured, jobs, isPreviewConfigured);
                }
            }

            EnqueueJobs(jobs);
        }

        /// <summary>
        /// This seeds all descendants of a parent, when parent node has been renamed.
        /// The IsDirty method to check for name changes does not work if node is saved first and published after. That is why we are using the publishing event here
        /// since we can detect the name change comparing the version to be published, with the currently published version of the node. 
        /// In the EnterspeedContentCacheRefresherNotificationHandler we cannot compare with the published node, since the node we are publishing and comparing with
        /// is already a part of the publishing cache. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="content"></param>
        /// <param name="isPublishConfigured"></param>
        /// <param name="jobs"></param>
        /// <param name="isPreviewConfigured"></param>
        private void HandleParentNameChange(UmbracoContextReference context, IContent content, bool isPublishConfigured, ICollection<EnterspeedJob> jobs, bool isPreviewConfigured)
        {
            if (context.UmbracoContext.Content != null)
            {
                var currentlyPublishedContent = context.UmbracoContext.Content.GetById(content.Id);
                var nameHasChanged = !currentlyPublishedContent?.Name?.Equals(content.Name);
                if (nameHasChanged is not true) return;

                foreach (var descendant in currentlyPublishedContent.Descendants("*").ToList())
                {
                    var descendantCultures = descendant.ContentType.VariesByCulture()
                        ? _umbracoCultureProvider.GetCulturesForCultureVariant(descendant)
                        : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(descendant) };

                    foreach (var descendantCulture in descendantCultures)
                    {
                        if (isPublishConfigured)
                        {
                            jobs.Add(_enterspeedJobFactory.GetPublishJob(descendant, descendantCulture, EnterspeedContentState.Publish));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This only handles variants that has been unpublished. Publishing is handled in the ContentCacheUpdated method
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isPublishConfigured"></param>
        /// <param name="jobs"></param>
        /// <param name="isPreviewConfigured"></param>
        private void HandleUnpublishedVariants(IContent content, bool isPublishConfigured, List<EnterspeedJob> jobs, bool isPreviewConfigured)
        {
            if (content.ContentType.VariesByCulture())
            {
                List<IContent> descendants = null;

                foreach (var culture in _umbracoCultureProvider.GetCulturesForCultureVariant(content))
                {
                    var isCultureUnpublished =
                        content.IsPropertyDirty(ContentBase.ChangeTrackingPrefix.UnpublishedCulture + culture);
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
                            descendants = _contentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                        }

                        foreach (var descendant in descendants)
                        {
                            if (descendant.ContentType.VariesByCulture())
                            {
                                var descendantCultures =
                                    _umbracoCultureProvider.GetCulturesForCultureVariant(descendant);
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
    }
}