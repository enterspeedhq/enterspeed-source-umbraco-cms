using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.NetCore.Data.Models;
using Enterspeed.Source.UmbracoCms.NetCore.Data.Repositories;
using Enterspeed.Source.UmbracoCms.NetCore.Factories;
using Enterspeed.Source.UmbracoCms.NetCore.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
#if U9
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.NetCore.NotificationHandlers
{
    public class EnterspeedMediaItemSavedEventHandler : BaseEnterspeedNotificationHandler, INotificationHandler<MediaSavedNotification>
    {
        private const int IndexPageSize = 9999;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IMediaService _mediaService;

        public EnterspeedMediaItemSavedEventHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService,
            IMediaService mediaService)
                : base(
            configurationService,
            enterspeedJobRepository,
            enterspeedJobsHandlingService,
            umbracoContextFactory,
            scopeProvider,
            auditService)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
            _mediaService = mediaService;
        }

        public void Handle(MediaSavedNotification notification)
        {
            MediaSaved(notification);
        }

        private void MediaSaved(MediaSavedNotification notification)
        {
            var isPublishConfigured = _configurationService.IsPublishConfigured();

            if (!isPublishConfigured)
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();

            foreach (var mediaItem in notification.SavedEntities)
            {
                if (mediaItem.ContentType.Alias.Equals("Folder"))
                {
                    var mediaItems = _mediaService.GetPagedDescendants(mediaItem.Id, 0, IndexPageSize, out var totalRecords).ToList();
                    if (totalRecords > 0)
                    {
                        foreach (var item in mediaItems)
                        {
                            jobs.Add(_enterspeedJobFactory.GetPublishJob(item, string.Empty, EnterspeedContentState.Publish));
                        }
                    }
                }

                jobs.Add(_enterspeedJobFactory.GetPublishJob(mediaItem, string.Empty, EnterspeedContentState.Publish));
            }

            EnqueueJobs(jobs);
        }
    }
}