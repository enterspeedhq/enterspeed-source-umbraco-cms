using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Factories;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.V9.NotificationHandlers
{
    public class EnterspeedMediaMovedEventHandler : BaseEnterspeedNotificationHandler, INotificationHandler<MediaMovedNotification>
    {
        private const int IndexPageSize = 9999;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IMediaService _mediaService;

        public EnterspeedMediaMovedEventHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IAuditService auditService,
            IEnterspeedJobFactory enterspeedJobFactory,
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

        public void Handle(MediaMovedNotification notification) => MediaMoved(notification);

        private void MediaMoved(MediaMovedNotification notification)
        {
            var isPublishConfigured = _configurationService.IsPublishConfigured();

            if (!isPublishConfigured)
            {
                return;
            }

            var entities = notification.MoveInfoCollection.Select(ei => ei.Entity).ToList();
            var jobs = new List<EnterspeedJob>();

            foreach (var mediaItem in entities)
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