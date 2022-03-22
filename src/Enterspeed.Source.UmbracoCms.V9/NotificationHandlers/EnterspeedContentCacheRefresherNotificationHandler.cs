using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.NotificationHandlers
{
    public class EnterspeedContentCacheRefresherNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<ContentCacheRefresherNotification>
    {
        public EnterspeedContentCacheRefresherNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandler enterspeedJobHandler,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider)
            : base(
                  configurationService, 
                  enterspeedJobRepository, 
                  enterspeedJobHandler,
                  umbracoContextFactory, 
                  scopeProvider)
        {
        }
        
        public void Handle(ContentCacheRefresherNotification notification)
        {
            if (!IsConfigured())
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
    }
}