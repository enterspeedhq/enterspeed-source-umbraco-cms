using System.Collections.Generic;
using System.Linq;
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
                                var publishedUpdateDate = publishedNode.CultureDate(culture);
                                var savedUpdateDate = savedNode.CultureDate(culture);

                                if (savedUpdateDate > publishedUpdateDate)
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
    }
}