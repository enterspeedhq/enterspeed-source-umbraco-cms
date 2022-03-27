using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Enterspeed.Source.UmbracoCms.V7.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V7.Factories;
using Enterspeed.Source.UmbracoCms.V7.Models.Api;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services
{
    public class EnterspeedJobService
    {
        private readonly EnterspeedJobRepository _enterspeedJobRepository;
        private readonly EnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedJobService()
        {
            _enterspeedJobRepository = EnterspeedContext.Current.Repositories.JobRepository;
            _enterspeedJobFactory = EnterspeedContext.Current.Services.JobFactory;
        }

        public SeedResponse Seed(bool publish, bool preview)
        {
            var contentJobs = new List<EnterspeedJob>();

            var publishContentCount = 0;
            var previewContentCount = 0;
            if (publish)
            {
                var publishContentJobs = GetPublishContentJobs(out publishContentCount);
                if (publishContentJobs.Count >= 1)
                {
                    contentJobs.AddRange(publishContentJobs);
                }
            }

            if (preview)
            {
                var previewContentJobs = GetPreviewContentJobs(out previewContentCount);
                if (previewContentJobs.Count >= 1)
                {
                    contentJobs.AddRange(previewContentJobs);
                }
            }

            var dictionaryJobs = GetDictionaryJobs(publish, preview, out var dictionaryCount);
            var jobs = contentJobs.Union(dictionaryJobs).ToList();
            _enterspeedJobRepository.Save(jobs);

            return new SeedResponse
            {
                ContentCount = publishContentCount + previewContentCount,
                DictionaryCount = dictionaryCount,
                JobsAdded = jobs.Count
            };
        }

        private List<EnterspeedJob> GetPublishContentJobs(out int nodesCount)
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
                jobs.Add(_enterspeedJobFactory.GetPublishJob(content, EnterspeedContentState.Publish));
            }

            return jobs;
        }

        private List<EnterspeedJob> GetPreviewContentJobs(out int nodesCount)
        {
            var jobs = new List<EnterspeedJob>();

            var rootContent = ApplicationContext.Current.Services.ContentService.GetRootContent().ToList();
            var rootContentDescendants = rootContent.SelectMany(x => x.Descendants()).ToList();

            var allContent = rootContent.Union(rootContentDescendants).ToList();

            nodesCount = allContent.Count;
            if (!allContent.Any())
            {
                return jobs;
            }

            foreach (var content in allContent)
            {
                jobs.Add(_enterspeedJobFactory.GetPublishJob(content, EnterspeedContentState.Preview));
            }

            return jobs;
        }

        private IEnumerable<EnterspeedJob> GetDictionaryJobs(bool publish, bool preview, out int dictionaryCount)
        {
            var jobs = new List<EnterspeedJob>();
            dictionaryCount = 0;

            if (!publish && !preview)
            {
                return jobs;
            }

            var allDictionaryItems =
                ApplicationContext.Current.Services.LocalizationService.GetDictionaryItemDescendants(null)
                    .ToList();

            foreach (var dictionaryItem in allDictionaryItems)
            {
                foreach (var translation in dictionaryItem.Translations)
                {
                    if (publish)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Publish));
                    }

                    if (preview)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Preview));
                    }
                }
            }

            dictionaryCount = allDictionaryItems.Count;

            return jobs;
        }
    }
}
