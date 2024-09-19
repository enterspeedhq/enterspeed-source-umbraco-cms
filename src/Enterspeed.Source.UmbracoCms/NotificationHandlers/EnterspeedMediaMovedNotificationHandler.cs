using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Web;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif
namespace Enterspeed.Source.UmbracoCms.Base.NotificationHandlers
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
            IMediaService mediaService,
            IServerRoleAccessor serverRoleAccessor,
            ILogger<EnterspeedMediaMovedEventHandler> logger)
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
            _mediaService = mediaService;
        }

        public void Handle(MediaMovedNotification notification) => MediaMoved(notification);

        private void MediaMoved(MediaMovedNotification notification)
        {
            var isPublishConfigured = IsPublishConfigured();
            var isPreviewConfigured = IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
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
                            if (isPublishConfigured)
                            {
                                jobs.Add(_enterspeedJobFactory.GetPublishJob(item, string.Empty, EnterspeedContentState.Publish));
                            }

                            if (isPreviewConfigured)
                            {
                                jobs.Add(_enterspeedJobFactory.GetPublishJob(item, string.Empty, EnterspeedContentState.Preview));
                            }
                        }
                    }
                }

                if (isPublishConfigured)
                {
                    jobs.Add(_enterspeedJobFactory.GetPublishJob(mediaItem, string.Empty, EnterspeedContentState.Publish));
                }

                if (isPreviewConfigured)
                {
                    jobs.Add(_enterspeedJobFactory.GetPublishJob(mediaItem, string.Empty, EnterspeedContentState.Preview));
                }
            }

            EnqueueJobs(jobs);
        }
    }
}