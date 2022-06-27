using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Enterspeed.Source.UmbracoCms.V7.Factories;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.EventHandlers
{
    public class MediaEventHandler : ApplicationEventHandler
    {
        private static readonly EnterspeedJobFactory EnterspeedJobFactory = EnterspeedContext.Current.Services.JobFactory;

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            MediaService.Saved += MediaService_Saved;
            MediaService.Moved += MediaService_Moved;
            MediaService.Trashed += MediaService_Trashed;
        }

        private static void MediaService_Trashed(IMediaService sender, MoveEventArgs<IMedia> e)
        {
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();

            if (!isPublishConfigured)
            {
                return;
            }

            var entities = e.MoveInfoCollection.ToList();
            var jobs = new List<EnterspeedJob>();

            foreach (var mediaItem in entities.Select(ei => ei.Entity))
            {
                jobs.Add(EnterspeedJobFactory.GetDeleteJob(mediaItem, string.Empty, EnterspeedContentState.Publish));
            }

            EnqueueJobs(jobs);
        }

        private static void MediaService_Moved(IMediaService sender, MoveEventArgs<IMedia> e)
        {
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();

            if (!isPublishConfigured)
            {
                return;
            }

            var entities = e.MoveInfoCollection.ToList();
            var jobs = new List<EnterspeedJob>();

            foreach (var mediaItem in entities.Select(ei => ei.Entity))
            {
                if (mediaItem.ContentType.Alias.Equals("Folder"))
                {
                    var mediaItems = ApplicationContext.Current.Services.MediaService.GetPagedDescendants(mediaItem.Id, 0, 99999, out long totalRecords).ToList();
                    if (totalRecords > 0)
                    {
                        foreach (var item in mediaItems)
                        {
                            if (!item.ContentType.Alias.Equals("Folder"))
                            {
                                jobs.Add(EnterspeedJobFactory.GetPublishJob(item, string.Empty, EnterspeedContentState.Publish));
                            }
                        }
                    }
                }
                else
                {
                    jobs.Add(EnterspeedJobFactory.GetPublishJob(mediaItem, string.Empty, EnterspeedContentState.Publish));
                }
            }

            EnqueueJobs(jobs);
        }

        private static void MediaService_Saved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            var jobs = new List<EnterspeedJob>();
            var isPublishConfigured = EnterspeedContext.Current.Services.ConfigurationService.IsPublishConfigured();

            if (!isPublishConfigured)
            {
                return;
            }

            var entities = e.SavedEntities.ToList();

            foreach (var mediaItem in entities)
            {
                if (mediaItem.ContentType.Alias.Equals("Folder"))
                {
                    var mediaItems = ApplicationContext.Current.Services.MediaService.GetPagedDescendants(mediaItem.Id, 0, 99999, out long totalRecords).ToList();
                    if (totalRecords > 0)
                    {
                        foreach (var item in mediaItems)
                        {
                            if (!item.ContentType.Alias.Equals("Folder"))
                            {
                                jobs.Add(EnterspeedJobFactory.GetPublishJob(item, string.Empty, EnterspeedContentState.Publish));
                            }
                        }
                    }
                }
                else
                {
                    jobs.Add(EnterspeedJobFactory.GetPublishJob(mediaItem, string.Empty, EnterspeedContentState.Publish));
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
