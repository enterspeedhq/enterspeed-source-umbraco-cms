using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Models;
using Enterspeed.Source.UmbracoCms.Base.Models.Api;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Enterspeed.Source.UmbracoCms.Base.Services.DataProperties;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public class EnterspeedJobService : IEnterspeedJobService
    {
        private readonly IContentService _contentService;
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedDictionaryTranslation _enterspeedDictionaryTranslation;
        private readonly IMediaService _mediaService;
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IEnterspeedMasterContentService _enterspeedMasterContentService;

        public EnterspeedJobService(
            IContentService contentService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IUmbracoContextFactory umbracoContextFactory,
            ILocalizationService localizationService,
            IEnterspeedJobFactory enterspeedJobFactory,
            IEnterspeedMasterContentService enterspeedMasterContentService,
            IMediaService mediaService,
            IUmbracoCultureProvider umbracoCultureProvider,
            IEnterspeedDictionaryTranslation enterspeedDictionaryTranslation)
        {
            _contentService = contentService;
            _enterspeedJobRepository = enterspeedJobRepository;
            _umbracoContextFactory = umbracoContextFactory;
            _localizationService = localizationService;
            _enterspeedJobFactory = enterspeedJobFactory;
            _enterspeedMasterContentService = enterspeedMasterContentService;
            _mediaService = mediaService;
            _umbracoCultureProvider = umbracoCultureProvider;
            _enterspeedDictionaryTranslation = enterspeedDictionaryTranslation;
        }

        public SeedResponse Seed(bool publish, bool preview)
        {
            var contentJobs = GetContentJobs(publish, preview, out var contentCount);
            var dictionaryJobs = GetDictionaryJobs(publish, preview, out var dictionaryCount);
            var mediaJobs = GetMediaJobs(publish, preview, out var mediaCount);

            var jobs = contentJobs.Union(dictionaryJobs).Union(mediaJobs).ToList();

            if (_enterspeedMasterContentService.IsMasterContentEnabled())
            {
                jobs.AddRange(_enterspeedMasterContentService.CreatePublishMasterContentJobs(jobs));
            }

            _enterspeedJobRepository.Save(jobs);
            var numberOfPendingJobs = _enterspeedJobRepository.GetNumberOfPendingJobs();

            return new SeedResponse
            {
                ContentCount = contentCount,
                DictionaryCount = dictionaryCount,
                MediaCount = mediaCount,
                JobsAdded = jobs.Count,
                NumberOfPendingJobs = numberOfPendingJobs
            };
        }

        public SeedResponse CustomSeed(bool publish, bool preview, CustomSeed customSeed)
        {
            var includeAllContent = customSeed.ContentNodes.Any(x => x.Id == -1);
            var includeAllMedia = customSeed.MediaNodes.Any(x => x.Id == -1);
            var includeAllDictionary = customSeed.DictionaryNodes.Any(x => x.Id == -1);

            var contentCount = 0;
            var customSeedContentCount = 0;
            var contentJobs = includeAllContent
                ? GetContentJobs(publish, preview, out contentCount)
                : GetContentJobs(publish, preview, customSeed, out customSeedContentCount);

            var mediaCount = 0;
            var customSeedMediaCount = 0;
            var mediaJobs = includeAllMedia
                ? GetMediaJobs(publish, preview, out mediaCount)
                : GetMediaJobs(publish, preview, customSeed, out customSeedMediaCount);

            var dictionaryCount = 0;
            var customSeedDictionaryCount = 0;
            var dictionaryJobs = includeAllDictionary
                ? GetDictionaryJobs(publish, preview, out dictionaryCount)
                : GetDictionaryJobs(publish, preview, customSeed, out customSeedDictionaryCount);

            var jobs = contentJobs.Union(dictionaryJobs).Union(mediaJobs).ToList();

            if (_enterspeedMasterContentService.IsMasterContentEnabled())
            {
                jobs.AddRange(_enterspeedMasterContentService.CreatePublishMasterContentJobs(jobs));
            }

            _enterspeedJobRepository.Save(jobs);
            var numberOfPendingJobs = _enterspeedJobRepository.GetNumberOfPendingJobs();

            return new SeedResponse
            {
                ContentCount = includeAllContent ? contentCount : customSeedContentCount,
                MediaCount = includeAllMedia ? mediaCount : customSeedMediaCount,
                DictionaryCount = includeAllMedia ? dictionaryCount : customSeedDictionaryCount,
                JobsAdded = jobs.Count,
                NumberOfPendingJobs = numberOfPendingJobs
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

        private List<EnterspeedJob> GetContentJobs(bool publish, bool preview, CustomSeed customSeed, out int nodesCount)
        {
            var jobs = new List<EnterspeedJob>();
            nodesCount = 0;

            if (!publish && !preview)
            {
                return jobs;
            }

            var allContent = new List<IContent>();
            foreach (var contentSeedNode in customSeed.ContentNodes)
            {
                var contentNode = _contentService.GetById(contentSeedNode.Id);

                allContent.Add(contentNode);

                if (contentSeedNode.IncludeDescendants)
                {
                    var descendants = _contentService.GetPagedDescendants(contentSeedNode.Id, 0, int.MaxValue, out var total).ToList();

                    allContent.AddRange(descendants);
                }
            }

            allContent = allContent.DistinctBy(x => x.Id).ToList();

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

        private List<EnterspeedJob> GetJobsForContent(IContent content, UmbracoContextReference context, bool publish, bool preview)
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
                var culture = _umbracoCultureProvider.GetCultureForNonCultureVariant(content);
                if (publish && content.Published)
                {
                    culturesToPublish.Add(culture);
                }

                if (preview)
                {
                    culturesToPreview.Add(culture);
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
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, _enterspeedDictionaryTranslation.GetIsoCode(translation), EnterspeedContentState.Publish));
                    }

                    if (preview)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, _enterspeedDictionaryTranslation.GetIsoCode(translation), EnterspeedContentState.Preview));
                    }
                }
            }

            dictionaryCount = allDictionaryItems.Count;

            return jobs;
        }

        private IEnumerable<EnterspeedJob> GetDictionaryJobs(bool publish, bool preview, CustomSeed customSeed, out int dictionaryCount)
        {
            dictionaryCount = 0;
            var jobs = new List<EnterspeedJob>();

            if (!publish && !preview)
            {
                return jobs;
            }

            var allDictionaryItems = new List<IDictionaryItem>();
            foreach (var dictionarySeedNode in customSeed.DictionaryNodes)
            {
                var dictionaryItem = _localizationService.GetDictionaryItemById(dictionarySeedNode.Id);

                allDictionaryItems.Add(dictionaryItem);

                if (dictionarySeedNode.IncludeDescendants)
                {
                    var descendants = _localizationService.GetDictionaryItemDescendants(dictionaryItem.Key).ToList();

                    allDictionaryItems.AddRange(descendants);
                }
            }

            allDictionaryItems = allDictionaryItems.DistinctBy(x => x.Id).ToList();

            foreach (var dictionaryItem in allDictionaryItems)
            {
                foreach (var translation in dictionaryItem.Translations)
                {
                    if (publish)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, _enterspeedDictionaryTranslation.GetIsoCode(translation), EnterspeedContentState.Publish));
                    }

                    if (preview)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, _enterspeedDictionaryTranslation.GetIsoCode(translation), EnterspeedContentState.Preview));
                    }
                }
            }

            dictionaryCount = allDictionaryItems.Count;

            return jobs;
        }

        public IEnumerable<EnterspeedJob> GetMediaJobs(bool publish, bool preview, out int mediaCount)
        {
            mediaCount = 0;
            var jobs = new List<EnterspeedJob>();

            if (!publish && !preview)
            {
                return jobs;
            }

            var allMediaItems = _mediaService
                .GetPagedDescendants(-1, 0, int.MaxValue, out _)
                .Where(x => !x.ContentType.Alias.Equals("Folder"))
                .Where(x => !x.Trashed)
                .ToList();

            foreach (var media in allMediaItems)
            {
                if (publish)
                {
                    jobs.Add(_enterspeedJobFactory.GetPublishJob(media, string.Empty, EnterspeedContentState.Publish));
                }

                if (preview)
                {
                    jobs.Add(_enterspeedJobFactory.GetPublishJob(media, string.Empty, EnterspeedContentState.Preview));
                }
            }

            mediaCount = allMediaItems.Count;

            return jobs;
        }

        private IEnumerable<EnterspeedJob> GetMediaJobs(bool publish, bool preview, CustomSeed customSeed, out int mediaCount)
        {
            mediaCount = 0;
            var jobs = new List<EnterspeedJob>();

            if (!publish && !preview)
            {
                return jobs;
            }

            var allMediaItems = new List<IMedia>();
            foreach (var mediaSeedNode in customSeed.MediaNodes)
            {
                var mediaNode = _mediaService.GetById(mediaSeedNode.Id);

                allMediaItems.Add(mediaNode);

                if (mediaSeedNode.IncludeDescendants)
                {
                    var descendants = _mediaService
                        .GetPagedDescendants(mediaSeedNode.Id, 0, int.MaxValue, out _).ToList();

                    allMediaItems.AddRange(descendants);
                }
            }

            allMediaItems = allMediaItems.DistinctBy(x => x.Id)
                .Where(x => !x.ContentType.Alias.Equals("Folder"))
                .Where(x => !x.Trashed).ToList();

            foreach (var media in allMediaItems)
            {
                if (publish)
                {
                    jobs.Add(_enterspeedJobFactory.GetPublishJob(media, string.Empty, EnterspeedContentState.Publish));
                }

                if (preview)
                {
                    jobs.Add(_enterspeedJobFactory.GetPublishJob(media, string.Empty, EnterspeedContentState.Preview));
                }
            }

            mediaCount = allMediaItems.Count;

            return jobs;
        }
    }
}
