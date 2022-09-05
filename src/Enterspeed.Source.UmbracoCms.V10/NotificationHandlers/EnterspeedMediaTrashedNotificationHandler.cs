using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V10.Data.Models;
using Enterspeed.Source.UmbracoCms.V10.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V10.Factories;
using Enterspeed.Source.UmbracoCms.V10.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Enterspeed.Source.UmbracoCms.V10.NotificationHandlers
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
            IAuditService auditService)
                : base(
                    configurationService,
                    enterspeedJobRepository,
                    enterspeedJobsHandlingService,
                    umbracoContextFactory,
                    scopeProvider,
                    auditService)
        {
        }

        public void Handle(MediaMovedToRecycleBinNotification notification) => MediaServiceTrashed(notification);

        private void MediaServiceTrashed(MediaMovedToRecycleBinNotification notification)
        {
            var isPublishConfigured = _configurationService.IsPublishConfigured();
            if (!isPublishConfigured)
            {
                return;
            }

            var entities = notification.MoveInfoCollection.Select(c => c.Entity).ToList();
            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var mediaItem in entities)
                {
                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(mediaItem, string.Empty, EnterspeedContentState.Publish));
                }
            }

            EnqueueJobs(jobs);
        }
    }
}