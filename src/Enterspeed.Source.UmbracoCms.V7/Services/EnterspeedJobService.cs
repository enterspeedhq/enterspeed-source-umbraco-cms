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
            var contentJobs = GetContentJobs(out var contentCount);
            var dictionaryJobs = GetDictionaryJobs(out var dictionaryCount);

            var jobs = contentJobs.Union(dictionaryJobs).ToList();

            _enterspeedJobRepository.Save(jobs);

            return new SeedResponse
            {
                ContentCount = contentCount,
                DictionaryCount = dictionaryCount,
                JobsAdded = jobs.Count
            };
        }

        private List<EnterspeedJob> GetContentJobs(out int nodesCount)
        {
            var jobs = new List<EnterspeedJob>();
            var umbracoHelper = UmbracoContextHelper.GetUmbracoHelper();

            var allContent = umbracoHelper.TypedContentAtRoot()?.SelectMany(x => x.DescendantsOrSelf()).ToList() ?? new List<IPublishedContent>();

            nodesCount = allContent.Count;
            if (!allContent.Any())
            {
                return jobs;
            }

            foreach (var content in allContent)
            {
                var contentJob = GetJobForContent(content);
                if (contentJob == null)
                {
                    continue;
                }

                jobs.Add(contentJob);
            }

            return jobs;
        }

        private List<EnterspeedJob> GetDictionaryJobs(out int dictionaryCount)
        {
            var jobs = new List<EnterspeedJob>();
            var allDictionaryItems =
                ApplicationContext.Current.Services.LocalizationService.GetDictionaryItemDescendants(null)
                    .ToList();

            var now = DateTime.UtcNow;

            foreach (var dictionaryItem in allDictionaryItems)
            {
                foreach (var translation in dictionaryItem.Translations)
                {
                    jobs.Add(new EnterspeedJob
                    {
                        EntityId = dictionaryItem.Key.ToString(),
                        EntityType = EnterspeedJobEntityType.Dictionary,
                        Culture = translation.Language.IsoCode,
                        JobType = EnterspeedJobType.Publish,
                        State = EnterspeedJobState.Pending,
                        CreatedAt = now,
                        UpdatedAt = now,
                    });
                }
            }

            dictionaryCount = allDictionaryItems.Count;

            return jobs;
        }

        private EnterspeedJob GetJobForContent(IPublishedContent content)
        {
            var culture = content.GetCulture()?.ToString().ToLowerInvariant();

            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = content.Id.ToString(),
                Culture = culture,
                JobType = EnterspeedJobType.Publish,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                EntityType = EnterspeedJobEntityType.Content
            };
        }
    }
}
