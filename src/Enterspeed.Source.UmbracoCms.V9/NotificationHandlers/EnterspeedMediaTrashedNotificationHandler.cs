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
    public class EnterspeedMediaTrashedNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<MediaDeletedNotification>
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

        public void Handle(MediaDeletedNotification notification) => MediaService_Trashed(notification);

        private void MediaService_Trashed(MediaDeletedNotification notification)
        {
            var isPublishConfigured = _configurationService.IsPublishConfigured();
            if (!isPublishConfigured)
            {
                return;
            }

            var entities = notification.DeletedEntities.ToList();
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