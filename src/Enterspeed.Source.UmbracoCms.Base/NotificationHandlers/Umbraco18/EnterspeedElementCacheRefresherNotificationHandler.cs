#if UMBRACO_18_OR_GREATER
using System;
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
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Base.NotificationHandlers.Umbraco18
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
            //
            // A RefreshNode payload without culture information is a draft-only save:
            // publish and unpublish operations always carry PublishedCultures or
            // UnpublishedCultures ("*" for invariant elements), plain saves carry neither.
            // Draft saves only affect the preview source, so the publish source is left
            // untouched for them.
            var publishDocumentIds = new HashSet<int>();
            var previewDocumentIds = new HashSet<int>();
            var affectedCultures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var payload in jsonPayloads)
            {
                if ((payload.ChangeTypes & TreeChangeTypes.RefreshAll) == TreeChangeTypes.RefreshAll)
                {
                    // Bulk refresh: the payload carries no element id, so fan out to every
                    // document referencing any element - the same special case Umbraco's
                    // Delivery API output-cache eviction applies for RefreshAll
                    var allElementRelations = _relationService.GetByRelationTypeAlias(Constants.Conventions.RelationTypes.RelatedElementAlias)
                        ?? Enumerable.Empty<IRelation>();
                    foreach (var relation in allElementRelations)
                    {
                        publishDocumentIds.Add(relation.ParentId);
                        previewDocumentIds.Add(relation.ParentId);
                    }

                    continue;
                }

                var hasCultureSignal = (payload.PublishedCultures != null && payload.PublishedCultures.Any())
                    || (payload.UnpublishedCultures != null && payload.UnpublishedCultures.Any());
                var isDraftSaveOnly = payload.ChangeTypes == TreeChangeTypes.RefreshNode && !hasCultureSignal;

                foreach (var relation in _relationService.GetByChildId(payload.Id, Constants.Conventions.RelationTypes.RelatedElementAlias))
                {
                    previewDocumentIds.Add(relation.ParentId);

                    if (!isDraftSaveOnly)
                    {
                        publishDocumentIds.Add(relation.ParentId);
                    }
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

            if (!publishDocumentIds.Any() && !previewDocumentIds.Any())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();

            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var umb = context.UmbracoContext;

                // Unpublished pages are not ingested, so only fan out to published
                // referencing pages for the publish state. Ingesting re-resolves the
                // elements from the published element cache, so draft element content
                // can never end up in the publish source.
                if (isPublishConfigured)
                {
                    foreach (var documentId in publishDocumentIds)
                    {
                        var publishedNode = umb.Content.GetById(documentId);
                        if (publishedNode == null)
                        {
                            continue;
                        }

                        var cultures = publishedNode.ContentType.VariesByCulture()
                            ? _umbracoCultureProvider.GetCulturesForCultureVariant(publishedNode)
                            : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(publishedNode) };

                        foreach (var culture in FilterAffectedCultures(cultures.ToList(), affectedCultures))
                        {
                            jobs.Add(_enterspeedJobFactory.GetPublishJob(publishedNode, culture, EnterspeedContentState.Publish));
                        }
                    }
                }

                if (isPreviewConfigured)
                {
                    foreach (var documentId in previewDocumentIds)
                    {
                        var savedNode = umb.Content.GetById(true, documentId);
                        if (savedNode == null)
                        {
                            continue;
                        }

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
