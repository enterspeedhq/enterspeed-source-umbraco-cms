using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Models.Api;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class EnterspeedJobService : IEnterspeedJobService
    {
        private readonly IContentService _contentService;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public EnterspeedJobService(
            IContentService contentService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IUmbracoContextFactory umbracoContextFactory)
        {
            _contentService = contentService;
            _enterspeedJobRepository = enterspeedJobRepository;
            _umbracoContextFactory = umbracoContextFactory;
        }

        public SeedResponse Seed()
        {
            var allContent = new List<IContent>();

            // Add all root nodes
            var rootContent = _contentService.GetRootContent().ToList();
            allContent.AddRange(rootContent);

            // Add all descendants
            foreach (var content in rootContent)
            {
                var descendants = _contentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out var total).ToList();
                allContent.AddRange(descendants);
            }

            var jobs = new List<EnterspeedJob>();
            if (!allContent.Any())
            {
                return new SeedResponse
                {
                    JobsAdded = 0,
                    Nodes = 0,
                };
            }

            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in allContent)
                {
                    var contentJobs = GetJobsForContent(content, context);
                    if (contentJobs == null || !contentJobs.Any())
                    {
                        continue;
                    }

                    jobs.AddRange(contentJobs);
                }
            }

            _enterspeedJobRepository.Save(jobs);

            return new SeedResponse
            {
                Nodes = allContent.Count,
                JobsAdded = jobs.Count
            };
        }

        private List<EnterspeedJob> GetJobsForContent(IContent content, UmbracoContextReference context)
        {
            var jobs = new List<EnterspeedJob>();

            var culturesToPublish = new List<string>();
            if (content.ContentType.VariesByCulture())
            {
                culturesToPublish = content.PublishedCultures.ToList();
            }
            else
            {
                var defaultCulture = GetDefaultCulture(context);
                if (content.Published)
                {
                    culturesToPublish.Add(defaultCulture);
                }
            }

            var now = DateTime.UtcNow;
            foreach (var culture in culturesToPublish)
            {
                jobs.Add(new EnterspeedJob
                {
                    ContentId = content.Id,
                    Culture = culture,
                    JobType = EnterspeedJobType.Publish,
                    State = EnterspeedJobState.Pending,
                    CreatedAt = now,
                    UpdatedAt = now,
                });
            }

            return jobs;
        }

        private string GetDefaultCulture(UmbracoContextReference context)
        {
            return context.UmbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }
    }
}
