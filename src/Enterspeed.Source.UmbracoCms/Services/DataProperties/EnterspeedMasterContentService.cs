﻿using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Extensions;
using Enterspeed.Source.UmbracoCms.Factories;

namespace Enterspeed.Source.UmbracoCms.Services.DataProperties
{
    public class EnterspeedMasterContentService : IEnterspeedMasterContentService
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public EnterspeedMasterContentService(IEnterspeedJobFactory enterspeedJobFactory, IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        public virtual bool IsMasterContentEnabled()
        {
            return _enterspeedConfigurationService.IsMasterContentDisabled();
        }

        public virtual List<EnterspeedJob> CreatePublishMasterContentJobs(List<EnterspeedJob> variantJobs)
        {
            var jobs = new List<EnterspeedJob>();

            var uniqueContentVariantJobs = GetUniqueContentVariantJobs(variantJobs);
            if (!uniqueContentVariantJobs.Any())
            {
                return jobs;
            }

            foreach (var uniqueContentVariantJob in uniqueContentVariantJobs)
            {
                if (uniqueContentVariantJob.JobType == EnterspeedJobType.Publish)
                {
                    jobs.Add(_enterspeedJobFactory.GetPublishMasterContentJob(uniqueContentVariantJob.EntityId, string.Empty, uniqueContentVariantJob.ContentState));
                }
            }

            return jobs;
        }

        public virtual List<EnterspeedJob> CreatePublishMasterContentJobs(string[] entityIds)
        {
            var jobs = new List<EnterspeedJob>();

            foreach (var entityId in entityIds)
            {
                jobs.Add(_enterspeedJobFactory.GetPublishMasterContentJob(entityId, string.Empty, EnterspeedContentState.Publish));
            }

            return jobs;
        }

        public virtual List<EnterspeedJob> CreateDeleteMasterContentJobs(List<EnterspeedJob> variantJobs)
        {
            var jobs = new List<EnterspeedJob>();

            var uniqueContentVariantJobs = GetUniqueContentVariantJobs(variantJobs);
            if (!uniqueContentVariantJobs.Any())
            {
                return jobs;
            }

            foreach (var uniqueContentVariantJob in uniqueContentVariantJobs)
            {
                if(uniqueContentVariantJob.JobType == EnterspeedJobType.Delete)
                {
                    jobs.Add(_enterspeedJobFactory.GetDeleteMasterContentJob(uniqueContentVariantJob.EntityId, string.Empty, uniqueContentVariantJob.ContentState));
                }
            }

            return jobs;
        }

        protected virtual List<EnterspeedJob> GetUniqueContentVariantJobs(List<EnterspeedJob> jobs)
        {
            var previewContentJobs = jobs
                .Where(x => x.EntityType == EnterspeedJobEntityType.Content && x.ContentState == EnterspeedContentState.Preview)
                .DistinctByProperty(x => x.EntityId)
                .ToList();

            var publishContentJobs = jobs
                .Where(x => x.EntityType == EnterspeedJobEntityType.Content && x.ContentState == EnterspeedContentState.Publish)
                .DistinctByProperty(x => x.EntityId)
                .ToList();

            var uniqueContentVariantJobs = new List<EnterspeedJob>();
            uniqueContentVariantJobs.AddRange(previewContentJobs);
            uniqueContentVariantJobs.AddRange(publishContentJobs);

            return uniqueContentVariantJobs;
        }
    }
}
