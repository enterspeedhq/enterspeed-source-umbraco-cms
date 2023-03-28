using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Factories;
using Enterspeed.Source.UmbracoCms.Providers;
using Enterspeed.Source.UmbracoCms.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.NotificationHandlers
{
    /// <summary>
    /// Sort notification handler for Umbraco 9 only as Umbraco 9 contains a bug in regards to AuditType.Sort not being set correct on the nodes being sorted, see https://github.com/umbraco/Umbraco-CMS/issues/13977
    /// For other Umbraco versions the sorting is handled by <see cref="EnterspeedContentCacheRefresherNotificationHandler"/>
    /// </summary>
    public class EnterspeedContentSortedNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<ContentSortedNotification>
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;

        public EnterspeedContentSortedNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService,
            IUmbracoCultureProvider umbracoCultureProvider)
            : base(
                configurationService,
                enterspeedJobRepository,
                enterspeedJobsHandlingService,
                umbracoContextFactory,
                scopeProvider,
                auditService)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
        }

        public void Handle(ContentSortedNotification notification)
        {
            ContentServiceSorting(notification.SortedEntities.ToList());
        }

        private void ContentServiceSorting(List<IContent> entities)
        {
            var isPublishConfigured = IsPublishConfigured();

            if (!isPublishConfigured)
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            foreach (var content in entities)
            {
                if (content.PublishedState != PublishedState.Published)
                {
                    continue;
                }

                var cultures = content.ContentType.VariesByCulture()
                    ? _umbracoCultureProvider.GetCulturesForCultureVariant(content)
                    : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(content) };

                foreach (var culture in cultures)
                {
                    jobs.Add(_enterspeedJobFactory.GetPublishJob(content, culture, EnterspeedContentState.Publish));
                }
            }

            EnqueueJobs(jobs);
        }
    }
}
