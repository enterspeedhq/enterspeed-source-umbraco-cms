using System;
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
    public class EnterspeedContentTrashingEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;

        public EnterspeedContentTrashingEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IUmbracoCultureProvider umbracoCultureProvider,
            IRuntimeState runtime,
            ILogger logger)
            : base(umbracoContextFactory, enterspeedJobRepository, jobsHandlingService, configurationService, scopeProvider, runtime, logger)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
            _umbracoCultureProvider = umbracoCultureProvider;
        }

        public void Initialize()
        {
            ContentService.Trashing += ContentServiceTrashing;
        }

        public void ContentServiceTrashing(IContentService sender, MoveEventArgs<IContent> e)
        {
            if (!ConfigurationService.IsPublishConfigured() && !ConfigurationService.IsPreviewConfigured())
            {
                return;
            }

            var entities = e.MoveInfoCollection.Select(x => x.Entity).ToList();

            if (!entities.Any())
            {
                return;
            }

            var isPublishConfigured = ConfigurationService.IsPublishConfigured();
            var isPreviewConfigured = ConfigurationService.IsPreviewConfigured();

            var jobs = new List<EnterspeedJob>();
            using (var context = UmbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    var cultures = content.ContentType.VariesByCulture()
                        ? _umbracoCultureProvider.GetCulturesForCultureVariant(content)
                        : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(content) };

                    List<IContent> descendants = null;
                    foreach (var culture in cultures)
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
                            descendants = Current.Services.ContentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                        }

                        foreach (var descendant in descendants)
                        {
                            var descendantCultures = descendant.ContentType.VariesByCulture()
                                ? _umbracoCultureProvider.GetCulturesForCultureVariant(descendant)
                                : new List<string> { _umbracoCultureProvider.GetCultureForNonCultureVariant(descendant) };
                            foreach (var descendantCulture in descendantCultures)
                            {
                                if (isPublishConfigured)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, descendantCulture, EnterspeedContentState.Publish));
                                }

                                if (isPreviewConfigured)
                                {
                                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, descendantCulture, EnterspeedContentState.Preview));
                                }
                            }
                        }
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        public void Terminate()
        {
        }
    }
}