using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Enterspeed.Source.UmbracoCms.V7.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V7.Extensions;
using Enterspeed.Source.UmbracoCms.V7.Models.Api;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Models;

namespace Enterspeed.Source.UmbracoCms.V7.Services
{
    public class EnterspeedJobService
    {
        private readonly IContentService _contentService;
        private readonly EnterspeedJobRepository _enterspeedJobRepository;

        public EnterspeedJobService()
        {
            _contentService = ApplicationContext.Current.Services.ContentService;
            _enterspeedJobRepository = EnterspeedContext.Current.Repositories.JobRepository;
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
                var descendants = _contentService.GetPagedDescendants(content.Id, 0L, int.MaxValue, out var total).ToList();
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

            UmbracoContextHelper.EnsureUmbracoContext();

            foreach (var content in allContent)
            {
                var contentJob = GetJobForContent(content);
                if (contentJob == null)
                {
                    continue;
                }

                jobs.Add(contentJob);
            }

            _enterspeedJobRepository.Save(jobs);

            return new SeedResponse
            {
                Nodes = allContent.Count,
                JobsAdded = jobs.Count
            };
        }

        private EnterspeedJob GetJobForContent(IContent content)
        {
            var culture = content.GetCulture()?.ToString().ToLowerInvariant();

            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                ContentId = content.Id,
                Culture = culture,
                JobType = EnterspeedJobType.Publish,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
            };
        }
    }
}
