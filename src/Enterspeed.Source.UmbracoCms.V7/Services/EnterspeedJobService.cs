using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Enterspeed.Source.UmbracoCms.V7.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V7.Models.Api;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

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
            var umbracoHelper = UmbracoContextHelper.GetUmbracoHelper();

            var allContent = umbracoHelper.TypedContentAtRoot()?.SelectMany(x => x.DescendantsOrSelf()).ToList();

            if (allContent == null || !allContent.Any())
            {
                return new SeedResponse
                {
                    JobsAdded = 0,
                    Nodes = 0,
                };
            }

            var jobs = new List<EnterspeedJob>();
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

        private EnterspeedJob GetJobForContent(IPublishedContent content)
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
