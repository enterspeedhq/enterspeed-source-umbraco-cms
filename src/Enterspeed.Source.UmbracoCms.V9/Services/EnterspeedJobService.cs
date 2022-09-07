using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Factories;
using Enterspeed.Source.UmbracoCms.V9.Models.Api;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public class EnterspeedJobService : IEnterspeedJobService
    {
        private readonly IContentService _contentService;
        private readonly ILocalizationService _localizationService;
        private readonly IMediaService _mediaService;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedJobService(
            IContentService contentService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IUmbracoContextFactory umbracoContextFactory,
            ILocalizationService localizationService,
            IEnterspeedJobFactory enterspeedJobFactory,
            IMediaService mediaService)
        {
            _contentService = contentService;
            _enterspeedJobRepository = enterspeedJobRepository;
            _umbracoContextFactory = umbracoContextFactory;
            _localizationService = localizationService;
            _enterspeedJobFactory = enterspeedJobFactory;
            _mediaService = mediaService;
        }

        public SeedResponse Seed(bool publish, bool preview)
        {
            var contentJobs = GetContentJobs(publish, preview, out var contentCount);
            var dictionaryJobs = GetDictionaryJobs(publish, preview, out var dictionaryCount);
            var mediaJobs = GetMediaJobs(out var mediaCount);

            var jobs = contentJobs.Union(dictionaryJobs).Union(mediaJobs).ToList();

            _enterspeedJobRepository.Save(jobs);

            return new SeedResponse
            {
                ContentCount = contentCount,
                DictionaryCount = dictionaryCount,
                MediaCount = mediaCount,
                JobsAdded = jobs.Count
            };
        }

        private List<EnterspeedJob> GetContentJobs(bool publish, bool preview, out int nodesCount)
        {
            var jobs = new List<EnterspeedJob>();
            nodesCount = 0;

            if (!publish && !preview)
            {
                return jobs;
            }

            var allContent = new List<IContent>();

            // Add all root nodes
            var rootContent = _contentService.GetRootContent().ToList();
            allContent.AddRange(rootContent);

            // Add all descendants
            foreach (var content in rootContent)
            {
                var descendants = _contentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out var total)
                    .ToList();
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
                    var contentJobs = GetJobsForContent(content, context, publish, preview);
                    if (contentJobs == null || !contentJobs.Any())
                    {
                        continue;
                    }

                    jobs.AddRange(contentJobs);
                }
            }

            return jobs;
        }

        private List<EnterspeedJob> GetJobsForContent(
            IContent content, UmbracoContextReference context, bool publish,
            bool preview)
        {
            var jobs = new List<EnterspeedJob>();

            var culturesToPublish = new List<string>();
            var culturesToPreview = new List<string>();
            if (content.ContentType.VariesByCulture())
            {
                if (publish)
                {
                    culturesToPublish = content.PublishedCultures.ToList();
                }

                if (preview)
                {
                    culturesToPreview = content.PublishedCultures.ToList();
                    if (content.EditedCultures != null)
                    {
                        culturesToPreview.AddRange(content.EditedCultures);
                    }
                }
            }
            else
            {
                var defaultCulture = GetDefaultCulture(context);
                if (publish && content.Published)
                {
                    culturesToPublish.Add(defaultCulture);
                }

                if (preview)
                {
                    culturesToPreview.Add(defaultCulture);
                }
            }

            foreach (var culture in culturesToPublish.Distinct())
            {
                jobs.Add(_enterspeedJobFactory.GetPublishJob(content, culture, EnterspeedContentState.Publish));
            }

            foreach (var culture in culturesToPreview.Distinct())
            {
                jobs.Add(_enterspeedJobFactory.GetPublishJob(content, culture, EnterspeedContentState.Preview));
            }

            return jobs;
        }

        private IEnumerable<EnterspeedJob> GetDictionaryJobs(bool publish, bool preview, out int dictionaryCount)
        {
            dictionaryCount = 0;
            var jobs = new List<EnterspeedJob>();

            if (!publish && !preview)
            {
                return jobs;
            }

            var allDictionaryItems = _localizationService.GetDictionaryItemDescendants(null).ToList();

            foreach (var dictionaryItem in allDictionaryItems)
            {
                foreach (var translation in dictionaryItem.Translations)
                {
                    if (publish)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, translation.Language.IsoCode,
                            EnterspeedContentState.Publish));
                    }

                    if (preview)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, translation.Language.IsoCode,
                            EnterspeedContentState.Preview));
                    }
                }
            }

            dictionaryCount = allDictionaryItems.Count;

            return jobs;
        }

        private string GetDefaultCulture(UmbracoContextReference context)
        {
            return context.UmbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }

        public IEnumerable<EnterspeedJob> GetMediaJobs(out long mediaCount)
        {
            mediaCount = 0;

            var jobs = new List<EnterspeedJob>();
            var allMediaItems = _mediaService.GetPagedDescendants(-1, 0, int.MaxValue, out long totalRecords).ToList();

            foreach (var media in allMediaItems)
            {
                if (!media.ContentType.Alias.Equals("Folder"))
                {
                    jobs.Add(_enterspeedJobFactory.GetPublishJob(media, string.Empty, EnterspeedContentState.Publish));
                }
            }

            mediaCount = totalRecords;

            return jobs;
        }
    }
}