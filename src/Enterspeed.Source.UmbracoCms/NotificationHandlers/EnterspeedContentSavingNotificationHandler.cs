using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Services;
using Enterspeed.Source.UmbracoCms.Factories;
using Enterspeed.Source.UmbracoCms.Providers;
using Umbraco.Cms.Core;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.NotificationHandlers
{
    public class EnterspeedContentSavingNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<ContentSavingNotification>
    {
        private readonly IContentService _contentService;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;

        public EnterspeedContentSavingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IContentService contentService,
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
            _contentService = contentService;
            _enterspeedJobFactory = enterspeedJobFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
        }

        public void Handle(ContentSavingNotification notification)
        {
            ContentServiceSaving(notification.SavedEntities.ToList());
        }

        private void ContentServiceSaving(List<IContent> entities)
        {
            var isPreviewConfigured = IsPreviewConfigured();

            if (!isPreviewConfigured)
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    HandleParentNameChange(context, content, jobs);
                }
            }

            EnqueueJobs(jobs);
        }

        /// <summary>
        /// This seeds all descendants of a parent, when parent has been renamed and saved.
        /// The logic is different in a publishing scenario. Check HandleParentNameChange of EnterspeedContentPublishingNotificationHandler.
        /// We are not able to detect the name change with IsPropertyDirty in the publish or content cache refresher notification, that is why the solutions for this problem are different
        /// depending on the event.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="content"></param>
        /// <param name="jobs"></param>
        private void HandleParentNameChange(UmbracoContextReference context, IContent content, ICollection<EnterspeedJob> jobs)
        {
            if (context.UmbracoContext.Content != null)
            {
                var isDirty = content.IsPropertyDirty("Name");
                if (isDirty is not true) return;

                foreach (var descendant in _contentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out _).ToList())
                {
                    var descendantCultures = descendant.ContentType.VariesByCulture()
                        ? _umbracoCultureProvider.GetCulturesForCultureVariant(descendant)
                        : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(descendant) };

                    foreach (var descendantCulture in descendantCultures)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(descendant, descendantCulture, EnterspeedContentState.Preview));
                    }
                }
            }
        }
    }
}