using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Factories;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.EventHandlers
{
    public class EnterspeedContentSavingEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public EnterspeedContentSavingEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider,
            IUmbracoCultureProvider umbracoCultureProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IRuntimeState runtime,
            ILogger logger)
            : base(
                umbracoContextFactory, enterspeedJobRepository, jobsHandlingService, configurationService, scopeProvider, runtime, logger)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
            _enterspeedJobFactory = enterspeedJobFactory;
        }

        public void Initialize()
        {
            ContentService.Saving += ContentServiceSaving;
        }

        public void ContentServiceSaving(IContentService contentService, ContentSavingEventArgs e)
        {
            var isPreviewConfigured = IsPreviewConfigured();
            if (!isPreviewConfigured)
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in e.SavedEntities.ToList())
                {
                    HandleParentNameChange(context, content, jobs, contentService);
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
        /// <param name="contentService"></param>
        private void HandleParentNameChange(UmbracoContextReference context, IContent content, ICollection<EnterspeedJob> jobs, IContentService contentService)
        {
            if (context.UmbracoContext.Content != null)
            {
                var isDirty = content.IsPropertyDirty("Name");
                if (!isDirty)
                {
                    return;
                }

                foreach (var descendant in contentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out _).ToList())
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


        public void Terminate()
        {
        }
    }
}