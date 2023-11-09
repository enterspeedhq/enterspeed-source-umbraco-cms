using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Factories;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services.Changes;
using Umbraco.Web;
using Umbraco.Web.Cache;

namespace Enterspeed.Source.UmbracoCms.V8.EventHandlers
{
    public class EnterspeedContentCacheRefresherEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;

        public EnterspeedContentCacheRefresherEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IUmbracoCultureProvider umbracoCultureProvider,
            IRuntimeState runtime,
            ILogger logger)
            : base(umbracoContextFactory, enterspeedJobRepository, jobsHandlingService, configurationService, scopeProvider, runtime, logger)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
        }

        public void Initialize()
        {
            ContentCacheRefresher.CacheUpdated += ContentCacheUpdated;
        }

        public void ContentCacheUpdated(ContentCacheRefresher sender, CacheRefresherEventArgs e)
        {
            var isPublishConfigured = ConfigurationService.IsPublishConfigured();
            var isPreviewConfigured = ConfigurationService.IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            var jsonPayloads = e.MessageObject as ContentCacheRefresher.JsonPayload[];

            if (jsonPayloads == null || !jsonPayloads.Any())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();

            using (var context = UmbracoContextFactory.EnsureUmbracoContext())
            {
                var umb = context.UmbracoContext;
                foreach (var payload in jsonPayloads)
                {
                    var node = umb.Content.GetById(payload.Id);
                    var savedNode = umb.Content.GetById(true, payload.Id);
                    if (node == null && savedNode == null)
                    {
                        continue;
                    }

                    if (node != null && isPublishConfigured)
                    {
                        var audit = Current.Services.AuditService.GetPagedItemsByEntity(payload.Id, 0, 2, out _)
                            .FirstOrDefault();

                        if (audit == null)
                        {
                            continue;
                        }

                        if (audit.AuditType.Equals(AuditType.PublishVariant)
                            || audit.AuditType.Equals(AuditType.Publish)
                            || audit.AuditType.Equals(AuditType.Move)
                            || audit.AuditType.Equals(AuditType.Sort))
                        {
                            var cultures = node.ContentType.VariesByCulture()
                                ? _umbracoCultureProvider.GetCulturesForCultureVariant(node)
                                : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(node) };

                            List<IPublishedContent> descendants = null;

                            foreach (var culture in cultures)
                            {
                                var publishedUpdateDate = node.CultureDate(culture);
                                var savedUpdateDate = savedNode.CultureDate(culture);

                                if (savedUpdateDate > publishedUpdateDate)
                                {
                                    // This means that the nodes was only saved, so we skip creating any jobs for this node and culture
                                    continue;
                                }

                                jobs.Add(_enterspeedJobFactory.GetPublishJob(node, culture, EnterspeedContentState.Publish));

                                if (payload.ChangeTypes == TreeChangeTypes.RefreshBranch)
                                {
                                    if (descendants == null)
                                    {
                                        descendants = node.Descendants("*").ToList();
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

        public void Terminate()
        {
        }
    }
}