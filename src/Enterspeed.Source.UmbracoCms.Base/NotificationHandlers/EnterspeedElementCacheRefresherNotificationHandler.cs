#if UMBRACO_18_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Base.NotificationHandlers
{
    /// <summary>
    /// When a library element changes, Umbraco does not fire ContentCacheRefresherNotification
    /// for the documents referencing it. This handler fans the element change out to the
    /// referencing documents via the automatic umbElement relations and reingests them —
    /// the same pattern Umbraco core uses for Delivery API output-cache eviction.
    /// </summary>
    public class EnterspeedElementCacheRefresherNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<ElementCacheRefresherNotification>
    {
        private readonly IRelationService _relationService;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;

        public EnterspeedElementCacheRefresherNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IRelationService relationService,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService,
            IUmbracoCultureProvider umbracoCultureProvider,
            IServerRoleAccessor serverRoleAccessor,
            ILogger<EnterspeedElementCacheRefresherNotificationHandler> logger)
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
            _relationService = relationService;
            _enterspeedJobFactory = enterspeedJobFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
        }

        public void Handle(ElementCacheRefresherNotification notification)
        {
            var isPublishConfigured = IsPublishConfigured();
            var isPreviewConfigured = IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            var jsonPayloads = notification.MessageObject as ElementCacheRefresher.JsonPayload[];
            if (jsonPayloads == null || !jsonPayloads.Any())
            {
                return;
            }

            // Resolve the documents referencing the changed elements (deduplicated across
            // payloads), and collect the cultures the element change affects so variant
            // pages are only reingested for those cultures.
            var referencingDocumentIds = new HashSet<int>();
            var affectedCultures = new HashSet<string>();
            foreach (var payload in jsonPayloads)
            {
                foreach (var relation in _relationService.GetByChildId(payload.Id, Constants.Conventions.RelationTypes.RelatedElementAlias))
                {
                    referencingDocumentIds.Add(relation.ParentId);
                }

                if (payload.PublishedCultures != null)
                {
                    affectedCultures.UnionWith(payload.PublishedCultures);
                }

                if (payload.UnpublishedCultures != null)
                {
                    affectedCultures.UnionWith(payload.UnpublishedCultures);
                }
            }

            if (!referencingDocumentIds.Any())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();

            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var umb = context.UmbracoContext;
                foreach (var documentId in referencingDocumentIds)
                {
                    var publishedNode = umb.Content.GetById(documentId);
                    var savedNode = umb.Content.GetById(true, documentId);

                    // Unpublished pages are not ingested, so only fan out to published
                    // referencing pages for the publish state. Ingesting re-resolves the
                    // elements from the published element cache, so draft element content
                    // can never end up in the publish source.
                    if (publishedNode != null && isPublishConfigured)
                    {
                        var cultures = publishedNode.ContentType.VariesByCulture()
                            ? _umbracoCultureProvider.GetCulturesForCultureVariant(publishedNode)
                            : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(publishedNode) };

                        foreach (var culture in FilterAffectedCultures(cultures.ToList(), affectedCultures))
                        {
                            jobs.Add(_enterspeedJobFactory.GetPublishJob(publishedNode, culture, EnterspeedContentState.Publish));
                        }
                    }

                    if (savedNode != null && isPreviewConfigured)
                    {
                        var cultures = savedNode.ContentType.VariesByCulture()
                            ? _umbracoCultureProvider.GetCulturesForCultureVariant(savedNode)
                            : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(savedNode) };

                        foreach (var culture in FilterAffectedCultures(cultures.ToList(), affectedCultures))
                        {
                            jobs.Add(_enterspeedJobFactory.GetPublishJob(savedNode, culture, EnterspeedContentState.Preview));
                        }
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        /// <summary>
        /// Limits the reingested cultures to the ones the element change affected. Falls back
        /// to all of the page's cultures when the payload carries no culture information
        /// (invariant elements) or when none of the affected cultures exist on the page
        /// (e.g. an invariant page referencing a variant element).
        /// </summary>
        internal static List<string> FilterAffectedCultures(List<string> pageCultures, HashSet<string> affectedCultures)
        {
            if (!affectedCultures.Any())
            {
                return pageCultures;
            }

            var intersection = pageCultures.Where(affectedCultures.Contains).ToList();
            return intersection.Any() ? intersection : pageCultures;
        }
    }
}
#endif
