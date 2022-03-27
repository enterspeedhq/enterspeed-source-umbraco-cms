using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Enterspeed.Source.UmbracoCms.V7.Factories;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Sync;
using Umbraco.Web;
using Umbraco.Web.Cache;

namespace Enterspeed.Source.UmbracoCms.V7.EventHandlers
{
    public class ContentEventHandler : ApplicationEventHandler
    {
        private static readonly EnterspeedJobFactory EnterspeedJobFactory = EnterspeedContext.Current.Services.JobFactory;
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            CacheRefresherBase<PageCacheRefresher>.CacheUpdated += CacheRefresherBaseOnCacheUpdated;
            ContentService.Moved += ContentServiceOnMoved;
            ContentService.UnPublishing += ContentServiceUnPublishing;
            ContentService.Trashing += ContentServiceTrashing;
            ContentService.Saved += ContentServiceSaved;
        }

        private static void ContentServiceTrashing(IContentService sender, MoveEventArgs<IContent> e)
        {
            // Do not configure if Enterspeed is not configured in Umbraco
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();
            var isPreviewConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPreviewConfigured();
            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            var entities = e.MoveInfoCollection.Select(x => x.Entity).ToList();
            HandleUnpublishing(entities, isPreviewConfigured);
        }

        private static void ContentServiceUnPublishing(Umbraco.Core.Publishing.IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            // Do not configure if Enterspeed is not configured in Umbraco
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();
            if (!isPublishConfigured)
            {
                return;
            }

            var entities = e.PublishedEntities.ToList();
            HandleUnpublishing(entities, false);
        }

        private static void CacheRefresherBaseOnCacheUpdated(PageCacheRefresher sender, CacheRefresherEventArgs cacheRefresherEventArgs)
        {
            // Do not configure if Enterspeed is not configured in Umbraco
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();
            if (!isPublishConfigured)
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
                        jobs.Add(EnterspeedJobFactory.GetPublishJob(refreshContent, EnterspeedContentState.Publish));
                    }

                    break;
                case MessageType.RefreshByInstance:
                    var node = umbracoHelper.TypedContent(((IContent)cacheRefresherEventArgs.MessageObject).Id);
                    if (node != null)
                    {
                        jobs.Add(EnterspeedJobFactory.GetPublishJob(node, EnterspeedContentState.Publish));
                    }

                    break;
            }

            EnqueueJobs(jobs);
        }

        private static void ContentServiceOnMoved(IContentService sender, MoveEventArgs<IContent> moveEventArgs)
        {
            // Do not configure if Enterspeed is not configured in Umbraco
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();
            if (!isPublishConfigured)
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
                    jobs.Add(EnterspeedJobFactory.GetDeleteJob(entity, EnterspeedContentState.Publish));

                    if (entity.Children().Any())
                    {
                        // Delete all descendants
                        var descendants = UmbracoContext.Current.Application.Services.ContentService.GetDescendants(entity.Id);
                        foreach (var content in descendants)
                        {
                            jobs.Add(EnterspeedJobFactory.GetDeleteJob(content, EnterspeedContentState.Publish));
                        }
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        private static void ContentServiceSaved(IContentService sender, SaveEventArgs<IContent> e)
        {
            // Do not configure if Enterspeed is not configured in Umbraco
            var isPreviewConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPreviewConfigured();
            if (!isPreviewConfigured)
            {
                return;
            }

            var entities = e.SavedEntities.ToList();
            var jobs = new List<EnterspeedJob>();

            foreach (var content in entities)
            {
                jobs.Add(EnterspeedJobFactory.GetPublishJob(content, EnterspeedContentState.Preview));
            }

            EnqueueJobs(jobs);
        }

        private static void HandleUnpublishing(List<IContent> entities, bool unpublishFromPreview)
        {
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();
            var isPreviewConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPreviewConfigured();
            if (!entities.Any() || (!isPublishConfigured && !isPreviewConfigured))
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;

                // Publish source job
                if (isPublishConfigured)
                {
                    jobs.Add(EnterspeedJobFactory.GetDeleteJob(entity, EnterspeedContentState.Publish));
                }

                if (unpublishFromPreview && isPreviewConfigured)
                {
                    jobs.Add(EnterspeedJobFactory.GetDeleteJob(entity, EnterspeedContentState.Preview));
                }

                if (entity.Children().Any())
                {
                    // Delete all descendants
                    var descendants = UmbracoContext.Current.Application.Services.ContentService.GetDescendants(entity.Id);
                    foreach (var content in descendants)
                    {
                        // Publish source job
                        if (isPublishConfigured)
                        {
                            jobs.Add(EnterspeedJobFactory.GetDeleteJob(content, EnterspeedContentState.Publish));
                        }

                        if (unpublishFromPreview && isPreviewConfigured)
                        {
                            jobs.Add(EnterspeedJobFactory.GetDeleteJob(content, EnterspeedContentState.Preview));
                        }
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
