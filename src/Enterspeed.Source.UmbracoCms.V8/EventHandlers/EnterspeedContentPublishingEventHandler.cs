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
    public class EnterspeedContentPublishingEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;

        public EnterspeedContentPublishingEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IUmbracoCultureProvider umbracoCultureProvider,
            IRuntimeState runtime,
            ILogger logger)
            : base(
                umbracoContextFactory, enterspeedJobRepository, jobsHandlingService, configurationService, scopeProvider, runtime, logger)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
        }

        public void Initialize()
        {
            ContentService.Publishing += ContentServicePublishing;
        }

        public void ContentServicePublishing(IContentService sender, ContentPublishingEventArgs e)
        {
            var isPublishConfigured = ConfigurationService.IsPublishConfigured();
            var isPreviewConfigured = ConfigurationService.IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            // This only handles variants that has been unpublished. Publishing is handled in the ContentCacheUpdated method
            var entities = e.PublishedEntities.ToList();
            var jobs = new List<EnterspeedJob>();
            using (var context = UmbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    HandleUnpublishedVariants(e, content, isPublishConfigured, jobs, isPreviewConfigured);
                    HandleParentNameChange(context, content, isPublishConfigured, jobs);
                }
            }

            EnqueueJobs(jobs);
        }

        /// <summary>
        /// This seeds all descendants of a parent, when parent node has been renamed.
        /// The IsDirty method to check for name changes does not work if node is saved first and published after. That is why we are using the publishing event here
        /// since we can detect the name change comparing the version to be published, with the currently published version of the node.
        /// In the EnterspeedContentCacheRefresherNotificationHandler we cannot compare with the published node, since the node we are publishing and comparing with
        /// is already a part of the publishing cache.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="content"></param>
        /// <param name="isPublishConfigured"></param>
        /// <param name="jobs"></param>
        private void HandleParentNameChange(
            UmbracoContextReference context,
            IContent content,
            bool isPublishConfigured,
            ICollection<EnterspeedJob> jobs)
        {
            if (context.UmbracoContext.Content != null)
            {
                var currentlyPublishedContent = context.UmbracoContext.Content.GetById(content.Id);
                var nameHasChanged = !currentlyPublishedContent?.Name?.Equals(content.Name);
                if (nameHasChanged.HasValue && !nameHasChanged.Value)
                {
                    return;
                }

                foreach (var descendant in currentlyPublishedContent.Descendants("*").ToList())
                {
                    var descendantCultures = descendant.ContentType.VariesByCulture()
                        ? _umbracoCultureProvider.GetCulturesForCultureVariant(descendant)
                        : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(descendant) };

                    foreach (var descendantCulture in descendantCultures)
                    {
                        if (isPublishConfigured)
                        {
                            jobs.Add(_enterspeedJobFactory.GetPublishJob(descendant, descendantCulture, EnterspeedContentState.Publish));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This only handles variants that has been unpublished. Publishing is handled in the ContentCacheUpdated method.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="content"></param>
        /// <param name="isPublishConfigured"></param>
        /// <param name="jobs"></param>
        /// <param name="isPreviewConfigured"></param>
        private void HandleUnpublishedVariants(
            ContentPublishingEventArgs e,
            IContent content,
            bool isPublishConfigured,
            List<EnterspeedJob> jobs,
            bool isPreviewConfigured)
        {
            if (!content.ContentType.VariesByCulture())
            {
                return;
            }

            List<IContent> descendants = null;

            foreach (var culture in _umbracoCultureProvider.GetCulturesForCultureVariant(content))
            {
                var isCultureUnpublished = e.IsUnpublishingCulture(content, culture);

                if (isCultureUnpublished)
                {
                    if (isPublishConfigured)
                    {
                        jobs.Add(_enterspeedJobFactory.GetDeleteJob(content, culture, EnterspeedContentState.Publish));
                    }

                    if (isPreviewConfigured)
                    {
                        jobs.Add(_enterspeedJobFactory.GetDeleteJob(content, culture, EnterspeedContentState.Preview));
                    }

                    if (descendants == null)
                    {
                        descendants = Current.Services.ContentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out _).ToList();
                    }

                    foreach (var descendant in descendants)
                    {
                        if (descendant.ContentType.VariesByCulture())
                        {
                            var descendantCultures = _umbracoCultureProvider.GetCulturesForCultureVariant(descendant);
                            if (descendantCultures.Contains(culture))
                            {
                                if (isPublishConfigured)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, culture, EnterspeedContentState.Publish));
                                }

                                if (isPreviewConfigured)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, culture, EnterspeedContentState.Preview));
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Terminate()
        {
        }
    }
}