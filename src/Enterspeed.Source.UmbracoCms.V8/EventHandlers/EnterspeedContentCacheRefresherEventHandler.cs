using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services.Changes;
using Umbraco.Web;
using Umbraco.Web.Cache;

namespace Enterspeed.Source.UmbracoCms.V8.EventHandlers
{
    public class EnterspeedContentCacheRefresherEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        public EnterspeedContentCacheRefresherEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider)
            : base(
                umbracoContextFactory, enterspeedJobRepository, jobsHandlingService, configurationService,
                scopeProvider)
        {
        }

        public void Initialize()
        {
            ContentCacheRefresher.CacheUpdated += ContentCacheUpdated;
        }

        public void ContentCacheUpdated(ContentCacheRefresher sender, CacheRefresherEventArgs e)
        {
            if (!IsConfigured())
            {
                return;
            }

            var jsonPayloads = e.MessageObject as ContentCacheRefresher.JsonPayload[];

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
                    var node = umb.Content.GetById(payload.Id);
                    var savedNode = umb.Content.GetById(true, payload.Id);
                    if (node == null || savedNode == null)
                    {
                        continue;
                    }

                    var cultures = node.ContentType.VariesByCulture()
                        ? node.Cultures.Keys
                        : new List<string> { GetDefaultCulture(context) };

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

                        var now = DateTime.UtcNow;
                        jobs.Add(
                            new EnterspeedJob
                            {
                                EntityId = node.Id.ToString(),
                                EntityType = EnterspeedJobEntityType.Content,
                                Culture = culture,
                                JobType = EnterspeedJobType.Publish,
                                State = EnterspeedJobState.Pending,
                                CreatedAt = now,
                                UpdatedAt = now,
                            });

                        if (payload.ChangeTypes == TreeChangeTypes.RefreshBranch)
                        {
                            if (descendants == null)
                            {
                                descendants = node.Descendants("*").ToList();
                            }

                            foreach (var descendant in descendants)
                            {
                                var descendantCultures = descendant.ContentType.VariesByCulture()
                                    ? descendant.Cultures.Keys
                                    : new List<string> { GetDefaultCulture(context) };

                                foreach (var descendantCulture in descendantCultures)
                                {
                                    jobs.Add(
                                        new EnterspeedJob
                                        {
                                            EntityId = descendant.Id.ToString(),
                                            EntityType = EnterspeedJobEntityType.Content,
                                            Culture = descendantCulture,
                                            JobType = EnterspeedJobType.Publish,
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

            EnqueueJobs(jobs);
        }


        public void Terminate()
        {
        }
    }
}