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
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public EnterspeedJobService(
            IContentService contentService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IUmbracoContextFactory umbracoContextFactory,
            ILocalizationService localizationService)
        {
            _contentService = contentService;
            _enterspeedJobRepository = enterspeedJobRepository;
            _umbracoContextFactory = umbracoContextFactory;
            _localizationService = localizationService;
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

            nodesCount = allContent.Count;
            if (!allContent.Any())
            {
                return jobs;
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

            return jobs;
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
                    EntityId = content.Id.ToString(),
                    EntityType = EnterspeedJobEntityType.Content,
                    Culture = culture,
                    JobType = EnterspeedJobType.Publish,
                    State = EnterspeedJobState.Pending,
                    CreatedAt = now,
                    UpdatedAt = now,
                });
            }

            return jobs;
        }

        private List<EnterspeedJob> GetDictionaryJobs(out int dictionaryCount)
        {
            var jobs = new List<EnterspeedJob>();
            var allDictionaryItems = _localizationService.GetDictionaryItemDescendants(null).ToList();

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

        private string GetDefaultCulture(UmbracoContextReference context)
        {
            return context.UmbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }
    }
}
