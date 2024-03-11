#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Core.Data.Models;
using Enterspeed.Source.UmbracoCms.Core.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Core.Factories;
using Enterspeed.Source.UmbracoCms.Core.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.Core.NotificationHandlers
{
    public class EnterspeedMediaTrashedNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<MediaMovedToRecycleBinNotification>
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedMediaTrashedNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IAuditService auditService,
            IEnterspeedJobFactory enterspeedJobFactory,
            IServerRoleAccessor serverRoleAccessor,
            ILogger<EnterspeedMediaTrashedNotificationHandler> logger)
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
        }

        public void Handle(MediaMovedToRecycleBinNotification notification) => MediaServiceTrashed(notification);

        private void MediaServiceTrashed(MediaMovedToRecycleBinNotification notification)
        {
            var isPublishConfigured = IsPublishConfigured();
            var isPreviewConfigured = IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            var entities = notification.MoveInfoCollection.Select(c => c.Entity).ToList();

            var jobs = new List<EnterspeedJob>();
            using (_umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var mediaItem in entities)
                {
                    if (isPreviewConfigured)
                    {
                        jobs.Add(_enterspeedJobFactory.GetDeleteJob(mediaItem, string.Empty, EnterspeedContentState.Preview));
                    }

                    if (isPublishConfigured)
                    {
                        jobs.Add(_enterspeedJobFactory.GetDeleteJob(mediaItem, string.Empty, EnterspeedContentState.Publish));
                    }
                }
            }

            EnqueueJobs(jobs);
        }
    }
}