using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Enterspeed.Source.UmbracoCms.Base.Services.DataProperties;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Changes;
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
    public class EnterspeedContentCacheRefresherNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<ContentCacheRefresherNotification>
    {
        // PublishedCultures/UnpublishedCultures were added to JsonPayload in a later Umbraco patch.
        // Use reflection so the package compiles against older minimum versions.
        private static readonly PropertyInfo _publishedCulturesProperty =
            typeof(ContentCacheRefresher.JsonPayload).GetProperty("PublishedCultures");
        private static readonly PropertyInfo _unpublishedCulturesProperty =
            typeof(ContentCacheRefresher.JsonPayload).GetProperty("UnpublishedCultures");

        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;
        private readonly IEnterspeedMasterContentService _enterspeedMasterContentService;

        public EnterspeedContentCacheRefresherNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService,
            IUmbracoCultureProvider umbracoCultureProvider,
            IServerRoleAccessor serverRoleAccessor,
            IEnterspeedMasterContentService enterspeedMasterContentService,
            ILogger<EnterspeedContentCacheRefresherNotificationHandler> logger)
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
            _enterspeedJobFactory = enterspeedJobFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
            _enterspeedMasterContentService = enterspeedMasterContentService;
        }

        public void Handle(ContentCacheRefresherNotification notification)
        {
            var isPublishConfigured = IsPublishConfigured();
            var isPreviewConfigured = IsPreviewConfigured();
            var masterVariantsToUpdateDoToUnpublishOfASingleLanguageVariant = new HashSet<string>();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            var jsonPayloads = notification.MessageObject as ContentCacheRefresher.JsonPayload[];
            if (jsonPayloads == null || !jsonPayloads.Any())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();

            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var umb = context.UmbracoContext;
                foreach (var payload in jsonPayloads)
                {
                    var publishedNode = umb.Content.GetById(payload.Id);
                    var savedNode = umb.Content.GetById(true, payload.Id);
                    if (publishedNode == null && savedNode == null)
                    {
                        continue;
                    }

                    if (publishedNode != null && isPublishConfigured)
                    {
                        var audit = _auditService.GetPagedItemsByEntity(payload.Id, 0, 2, out var totalLogs).FirstOrDefault();

                        if (audit == null)
                        {
                            continue;
                        }

                        if (audit.AuditType.Equals(AuditType.PublishVariant)
                                || audit.AuditType.Equals(AuditType.Publish)
                                || audit.AuditType.Equals(AuditType.Move)
                                || audit.AuditType.Equals(AuditType.Sort))
                        {
                            var cultures = publishedNode.ContentType.VariesByCulture()
                                ? _umbracoCultureProvider.GetCulturesForCultureVariant(publishedNode)
                                : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(publishedNode) };

                            List<IPublishedContent> descendants = null;

                            foreach (var culture in cultures)
                            {
                                if (IsSavedButNotPublished(publishedNode, savedNode, culture))
                                {
                                    // This means that the nodes was only saved, so we skip creating any jobs for this node and culture
                                    continue;
                                }

                                jobs.Add(_enterspeedJobFactory.GetPublishJob(publishedNode, culture, EnterspeedContentState.Publish));

                                if (payload.ChangeTypes == TreeChangeTypes.RefreshBranch)
                                {
                                    if (descendants == null)
                                    {
                                        descendants = publishedNode.Descendants("*").ToList();
                                    }

                                    foreach (var descendant in descendants)
                                    {
                                        var descendantCultures = descendant.ContentType.VariesByCulture()
                                            ? _umbracoCultureProvider.GetCulturesForCultureVariant(descendant)
                                            : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(descendant) };

                                        foreach (var descendantCulture in descendantCultures)
                                        {
                                            jobs.Add(_enterspeedJobFactory.GetPublishJob(descendant, descendantCulture, EnterspeedContentState.Publish));
                                        }
                                    }
                                }
                            }
                        }
                        else if (audit.AuditType == AuditType.SaveVariant)
                        {
                            // Umbraco Deploy fires SaveVariant instead of the normal publish notifications.
                            if (_publishedCulturesProperty != null)
                            {
                                // Umbraco 13.6+: payload carries explicit PublishedCultures/UnpublishedCultures,
                                // so we know exactly what changed without comparing node states.
                                var publishedCultures = GetPayloadCultures(_publishedCulturesProperty, payload);
                                foreach (var culture in publishedCultures)
                                {
                                    if (IsSavedButNotPublished(publishedNode, savedNode, culture))
                                    {
                                        continue;
                                    }

                                    jobs.Add(_enterspeedJobFactory.GetPublishJob(publishedNode, culture, EnterspeedContentState.Publish));
                                }

                                var unpublishedCultures = GetPayloadCultures(_unpublishedCulturesProperty, payload);
                                foreach (var culture in unpublishedCultures)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(publishedNode, culture, EnterspeedContentState.Publish));
                                }
                            }
                            else
                            {
                                // Fallback for Umbraco < 13.6: payload has no culture change info,
                                // so publish all currently published cultures and delete any that
                                // are in the draft but no longer published.
                                var cultures = publishedNode.ContentType.VariesByCulture()
                                    ? _umbracoCultureProvider.GetCulturesForCultureVariant(publishedNode)
                                    : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(publishedNode) };

                                foreach (var culture in cultures)
                                {
                                    if (IsSavedButNotPublished(publishedNode, savedNode, culture))
                                    {
                                        continue;
                                    }

                                    jobs.Add(_enterspeedJobFactory.GetPublishJob(publishedNode, culture, EnterspeedContentState.Publish));
                                }

                                AddDeleteJobsForUnpublishedCultures(publishedNode, savedNode, isPublishConfigured, isPreviewConfigured, jobs);
                            }
                        }
                        else if (audit.AuditType.Equals(AuditType.UnpublishVariant))
                        {
                            // If audit type is UnpublishVariant and we still have a publishedNode, then we know only the node is still published in another language
                            if (publishedNode != null && isPublishConfigured)
                            {
                                masterVariantsToUpdateDoToUnpublishOfASingleLanguageVariant.Add(payload.Id.ToString());
                            }
                        }
                    }

                    if (savedNode != null && isPreviewConfigured)
                    {
                        var cultures = savedNode.ContentType.VariesByCulture()
                            ? _umbracoCultureProvider.GetCulturesForCultureVariant(savedNode)
                            : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(savedNode) };

                        List<IPublishedContent> descendants = null;

                        foreach (var culture in cultures)
                        {
                            jobs.Add(_enterspeedJobFactory.GetPublishJob(savedNode, culture, EnterspeedContentState.Preview));

                            if (payload.ChangeTypes == TreeChangeTypes.RefreshBranch)
                            {
                                if (descendants == null)
                                {
                                    descendants = savedNode.Descendants("*").ToList();
                                }

                                foreach (var descendant in descendants)
                                {
                                    var descendantCultures = descendant.ContentType.VariesByCulture()
                                        ? _umbracoCultureProvider.GetCulturesForCultureVariant(descendant)
                                        : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(descendant) };

                                    foreach (var descendantCulture in descendantCultures)
                                    {
                                        jobs.Add(_enterspeedJobFactory.GetPublishJob(descendant, descendantCulture, EnterspeedContentState.Preview));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (_enterspeedMasterContentService.IsMasterContentEnabled())
            {
                jobs.AddRange(_enterspeedMasterContentService.CreatePublishMasterContentJobs(jobs));
                if (masterVariantsToUpdateDoToUnpublishOfASingleLanguageVariant.Any())
                {
                    jobs.AddRange(_enterspeedMasterContentService.CreatePublishMasterContentJobs(masterVariantsToUpdateDoToUnpublishOfASingleLanguageVariant.ToArray()));
                }
            }

            EnqueueJobs(jobs);
        }

        private static IEnumerable<string> GetPayloadCultures(PropertyInfo property, ContentCacheRefresher.JsonPayload payload)
            => property?.GetValue(payload) as IEnumerable<string> ?? Enumerable.Empty<string>();

        /// <summary>
        /// Returns true if the draft was saved more recently than the published version for the given culture,
        /// indicating the culture was saved but not yet published.
        /// </summary>
        private static bool IsSavedButNotPublished(IPublishedContent publishedNode, IPublishedContent savedNode, string culture)
            => savedNode?.CultureDate(culture) > publishedNode.CultureDate(culture);

        /// <summary>
        /// Creates delete jobs for culture variants that exist as drafts on the saved node
        /// but are no longer present in the published node. This handles cases where
        /// Umbraco Deploy transfers content with a mix of published and unpublished variants,
        /// bypassing the normal ContentPublishingNotification unpublish flow.
        /// Enterspeed delete operations are idempotent, so deleting a culture that was never
        /// synced is harmless.
        /// </summary>
        private void AddDeleteJobsForUnpublishedCultures(
            IPublishedContent publishedNode,
            IPublishedContent savedNode,
            bool isPublishConfigured,
            bool isPreviewConfigured,
            List<EnterspeedJob> jobs)
        {
            if (!publishedNode.ContentType.VariesByCulture() || savedNode == null)
            {
                return;
            }

            var publishedCultures = _umbracoCultureProvider.GetCulturesForCultureVariant(publishedNode).ToHashSet();

            foreach (var culture in _umbracoCultureProvider.GetCulturesForCultureVariant(savedNode))
            {
                if (publishedCultures.Contains(culture))
                {
                    continue;
                }

                if (isPublishConfigured)
                {
                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(publishedNode, culture, EnterspeedContentState.Publish));
                }

                if (isPreviewConfigured)
                {
                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(savedNode, culture, EnterspeedContentState.Preview));
                }
            }
        }
    }
}