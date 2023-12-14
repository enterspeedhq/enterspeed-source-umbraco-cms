using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Extensions;
using Enterspeed.Source.UmbracoCms.Factories;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.Services.DataProperties
{
    public class EnterspeedMasterContentService : IEnterspeedMasterContentService
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public EnterspeedMasterContentService(IUmbracoContextFactory umbracoContextFactory, IEnterspeedJobFactory enterspeedJobFactory, IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedJobFactory = enterspeedJobFactory;
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        public virtual bool IsMasterContentEnabled()
        {
            return _enterspeedConfigurationService.IsMasterContentDisabled();
        }

        public virtual List<EnterspeedJob> CreateMasterContentJobs(List<EnterspeedJob> variantJobs)
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
                else if(uniqueContentVariantJob.JobType == EnterspeedJobType.Delete && ShouldDeleteMasterContent(uniqueContentVariantJob))
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

        // TODO this approach is not working, we need to listen if the event fire is the ContentUnpublishingNotification
        protected virtual bool ShouldDeleteMasterContent(EnterspeedJob job)
        {
            using var context = _umbracoContextFactory.EnsureUmbracoContext();

            var isContentId = int.TryParse(job.EntityId, out var contentId);
            //var content = isContentId ? context.UmbracoContext.Content.GetById(job.ContentState == EnterspeedContentState.Preview, contentId) : null;
            var hasContent = isContentId && context.UmbracoContext.Content.HasById(job.ContentState == EnterspeedContentState.Preview, contentId);
            
            return !hasContent;
        }
    }
}
