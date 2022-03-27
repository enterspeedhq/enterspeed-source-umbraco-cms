using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.EventHandlers
{
    public class EnterspeedContentTrashingEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        public EnterspeedContentTrashingEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider)
            : base(
                umbracoContextFactory, enterspeedJobRepository, jobsHandlingService, configurationService,
                scopeProvider)
        {
        }

        public void Initialize()
        {
            ContentService.Trashing += ContentServiceTrashing;
        }


        public void ContentServiceTrashing(IContentService sender, MoveEventArgs<IContent> e)
        {
            var entities = e.MoveInfoCollection.Select(x => x.Entity).ToList();
            if (!IsConfigured())
            {
                return;
            }

            if (!entities.Any())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    var cultures = content.ContentType.VariesByCulture()
                        ? content.AvailableCultures
                        : new List<string> { GetDefaultCulture(context) };

                    List<IContent> descendants = null;
                    foreach (var culture in cultures)
                    {
                        var now = DateTime.UtcNow;
                        jobs.Add(
                            new EnterspeedJob
                            {
                                EntityId = content.Id.ToString(),
                                EntityType = EnterspeedJobEntityType.Content,
                                Culture = culture,
                                JobType = EnterspeedJobType.Delete,
                                State = EnterspeedJobState.Pending,
                                CreatedAt = now,
                                UpdatedAt = now,
                            });

                        if (descendants == null)
                        {
                            descendants = Current.Services.ContentService.GetPagedDescendants(
                                content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                        }

                        foreach (var descendant in descendants)
                        {
                            var descendantCultures = descendant.ContentType.VariesByCulture()
                                ? descendant.AvailableCultures
                                : new List<string> { GetDefaultCulture(context) };
                            foreach (var descendantCulture in descendantCultures)
                            {
                                jobs.Add(
                                    new EnterspeedJob
                                    {
                                        EntityId = descendant.Id.ToString(),
                                        EntityType = EnterspeedJobEntityType.Content,
                                        Culture = descendantCulture,
                                        JobType = EnterspeedJobType.Delete,
                                        State = EnterspeedJobState.Pending,
                                        CreatedAt = now,
                                        UpdatedAt = now,
                                    });
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