using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Sync;
using Umbraco.Web;
using Umbraco.Web.Cache;
using Umbraco.Web.Models;

namespace Enterspeed.Source.UmbracoCms.V7.EventHandlers
{
    public class ContentEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            CacheRefresherBase<PageCacheRefresher>.CacheUpdated += CacheRefresherBaseOnCacheUpdated;
            ContentService.Moved += ContentServiceOnMoved;
            ContentService.UnPublishing += ContentServiceUnPublishing;
            ContentService.Trashing += ContentServiceTrashing;
        }

        private static void ContentServiceTrashing(IContentService sender, MoveEventArgs<IContent> e)
        {
            // Do not configure if Enterspeed is not configured in Umbraco
            if (EnterspeedContext.Current?.Configuration == null
                || !EnterspeedContext.Current.Configuration.IsConfigured)
            {
                return;
            }

            var entities = e.MoveInfoCollection.Select(x => x.Entity).ToList();
            HandleUnpublishing(entities);
        }

        private static void ContentServiceUnPublishing(Umbraco.Core.Publishing.IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            // Do not configure if Enterspeed is not configured in Umbraco
            if (EnterspeedContext.Current?.Configuration == null
                || !EnterspeedContext.Current.Configuration.IsConfigured)
            {
                return;
            }

            var entities = e.PublishedEntities.ToList();
            HandleUnpublishing(entities);
        }

        private static void CacheRefresherBaseOnCacheUpdated(PageCacheRefresher sender, CacheRefresherEventArgs cacheRefresherEventArgs)
        {
            // Do not configure if Enterspeed is not configured in Umbraco
            if (EnterspeedContext.Current?.Configuration == null
                || !EnterspeedContext.Current.Configuration.IsConfigured)
            {
                return;
            }

            var umbracoHelper = UmbracoContextHelper.GetUmbracoHelper();
            var jobs = new List<EnterspeedJob>();

            switch (cacheRefresherEventArgs.MessageType)
            {
                case MessageType.RefreshById:
                    var refreshContent =
                        umbracoHelper.TypedContent((int)cacheRefresherEventArgs.MessageObject);
                    if (refreshContent != null)
                    {
                        var culture = refreshContent.GetCulture()?.IetfLanguageTag?.ToLowerInvariant();
                        var now = DateTime.UtcNow;
                        jobs.Add(new EnterspeedJob
                        {
                            ContentId = refreshContent.Id,
                            Culture = culture,
                            JobType = EnterspeedJobType.Publish,
                            State = EnterspeedJobState.Pending,
                            CreatedAt = now,
                            UpdatedAt = now,
                        });
                    }

                    break;
                case MessageType.RefreshByInstance:
                    var node = umbracoHelper.TypedContent(((IContent)cacheRefresherEventArgs.MessageObject).Id);
                    if (node != null)
                    {
                        var culture = node.GetCulture()?.IetfLanguageTag?.ToLowerInvariant();
                        var now = DateTime.UtcNow;
                        jobs.Add(new EnterspeedJob
                        {
                            ContentId = node.Id,
                            Culture = culture,
                            JobType = EnterspeedJobType.Publish,
                            State = EnterspeedJobState.Pending,
                            CreatedAt = now,
                            UpdatedAt = now,
                        });
                    }

                    break;
            }

            EnqueueJobs(jobs);
        }

        private static void ContentServiceOnMoved(IContentService sender, MoveEventArgs<IContent> moveEventArgs)
        {
            // Do not configure if Enterspeed is not configured in Umbraco
            if (EnterspeedContext.Current?.Configuration == null
                || !EnterspeedContext.Current.Configuration.IsConfigured)
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();

            foreach (var moveEventInfo in moveEventArgs.MoveInfoCollection)
            {
                var entity = moveEventInfo.Entity;

                // Deleted or unpublished
                if (moveEventInfo.NewParentId == -20 || !entity.Published)
                {
                    var now = DateTime.UtcNow;
                    jobs.Add(new EnterspeedJob
                    {
                        ContentId = entity.Id,
                        Culture = entity.GetCulture().IetfLanguageTag.ToLowerInvariant(),
                        JobType = EnterspeedJobType.Delete,
                        State = EnterspeedJobState.Pending,
                        CreatedAt = now,
                        UpdatedAt = now,
                    });

                    if (entity.Children().Any())
                    {
                        // Delete all descendants
                        var descendants = UmbracoContext.Current.Application.Services.ContentService.GetDescendants(entity.Id);
                        foreach (var content in descendants)
                        {
                            jobs.Add(new EnterspeedJob
                            {
                                ContentId = content.Id,
                                Culture = content.GetCulture().IetfLanguageTag.ToLowerInvariant(),
                                JobType = EnterspeedJobType.Delete,
                                State = EnterspeedJobState.Pending,
                                CreatedAt = now,
                                UpdatedAt = now,
                            });
                        }
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        private static void HandleUnpublishing(List<IContent> entities)
        {
            if (!entities.Any())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;
                jobs.Add(new EnterspeedJob
                {
                    ContentId = entity.Id,
                    Culture = entity.GetCulture().IetfLanguageTag.ToLowerInvariant(),
                    JobType = EnterspeedJobType.Delete,
                    State = EnterspeedJobState.Pending,
                    CreatedAt = now,
                    UpdatedAt = now,
                });

                if (entity.Children().Any())
                {
                    // Delete all descendants
                    var descendants = UmbracoContext.Current.Application.Services.ContentService.GetDescendants(entity.Id);
                    foreach (var content in descendants)
                    {
                        jobs.Add(new EnterspeedJob
                        {
                            ContentId = content.Id,
                            Culture = content.GetCulture().IetfLanguageTag.ToLowerInvariant(),
                            JobType = EnterspeedJobType.Delete,
                            State = EnterspeedJobState.Pending,
                            CreatedAt = now,
                            UpdatedAt = now,
                        });
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        private static void EnqueueJobs(List<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            EnterspeedContext.Current.Repositories.JobRepository.Save(jobs);
            EnterspeedContext.Current.Handlers.JobHandler.HandleJobs(jobs);
        }
    }
}
