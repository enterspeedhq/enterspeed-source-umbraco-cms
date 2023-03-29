using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Services;
using Enterspeed.Source.UmbracoCms.Factories;
using Enterspeed.Source.UmbracoCms.Providers;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.NotificationHandlers
{
    public class EnterspeedContentCacheRefresherNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<ContentCacheRefresherNotification>
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;

        public EnterspeedContentCacheRefresherNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService,
            IUmbracoCultureProvider umbracoCultureProvider)
            : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobsHandlingService,
                  umbracoContextFactory,
                  scopeProvider,
                  auditService)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
        }

        public void Handle(ContentCacheRefresherNotification notification)
        {
            var isPublishConfigured = IsPublishConfigured();
            var isPreviewConfigured = IsPreviewConfigured();

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

            EnqueueJobs(jobs);
        }
    }
}