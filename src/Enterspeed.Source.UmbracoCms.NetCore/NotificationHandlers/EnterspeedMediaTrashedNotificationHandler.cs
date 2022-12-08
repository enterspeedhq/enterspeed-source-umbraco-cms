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
            IEnterspeedJobFactory enterspeedJobFactory)
            : base(
                configurationService,
                enterspeedJobRepository,
                enterspeedJobsHandlingService,
                umbracoContextFactory,
                scopeProvider,
                auditService)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
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
            using (_umbracoContextFactory.EnsureUmbracoContext())
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