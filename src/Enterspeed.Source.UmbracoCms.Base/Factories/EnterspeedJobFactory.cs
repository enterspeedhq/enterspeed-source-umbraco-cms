using System;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Factories
{
    public class EnterspeedJobFactory : IEnterspeedJobFactory
    {
        public EnterspeedJob GetPublishJob(IPublishedContent content, string culture, EnterspeedContentState state)
        {
            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = content.Id.ToString(),
                EntityType = EnterspeedJobEntityType.Content,
                Culture = culture,
                JobType = EnterspeedJobType.Publish,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                ContentState = state
            };
        }

        public EnterspeedJob GetPublishJob(IContent content, string culture, EnterspeedContentState state)
        {
            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = content.Id.ToString(),
                EntityType = EnterspeedJobEntityType.Content,
                Culture = culture,
                JobType = EnterspeedJobType.Publish,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                ContentState = state
            };
        }

        public EnterspeedJob GetDeleteJob(IContent content, string culture, EnterspeedContentState state)
        {
            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = content.Id.ToString(),
                EntityType = EnterspeedJobEntityType.Content,
                Culture = culture,
                JobType = EnterspeedJobType.Delete,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                ContentState = state
            };
        }

        public EnterspeedJob GetPublishJob(IDictionaryItem dictionaryItem, string culture, EnterspeedContentState state)
        {
            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = dictionaryItem.Key.ToString(),
                EntityType = EnterspeedJobEntityType.Dictionary,
                Culture = culture,
                JobType = EnterspeedJobType.Publish,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                ContentState = state,
            };
        }

        public EnterspeedJob GetDeleteJob(IDictionaryItem dictionaryItem, string culture, EnterspeedContentState state)
        {
            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = dictionaryItem.Key.ToString(),
                EntityType = EnterspeedJobEntityType.Dictionary,
                Culture = culture,
                JobType = EnterspeedJobType.Delete,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                ContentState = state,
            };
        }

        public EnterspeedJob GetFailedJob(EnterspeedJob job, string exception)
        {
            return new EnterspeedJob
            {
                EntityId = job.EntityId,
                EntityType = job.EntityType,
                Culture = job.Culture,
                CreatedAt = job.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                JobType = job.JobType,
                State = EnterspeedJobState.Failed,
                ContentState = job.ContentState,
                Exception = exception
            };
        }

        public EnterspeedJob GetPublishJob(IMedia media, string culture, EnterspeedContentState state)
        {
            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = media.Id.ToString(),
                EntityType = EnterspeedJobEntityType.Media,
                Culture = culture,
                JobType = EnterspeedJobType.Publish,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                ContentState = state,
            };
        }

        public EnterspeedJob GetDeleteJob(IMedia media, string culture, EnterspeedContentState state)
        {
            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = media.Id.ToString(),
                EntityType = EnterspeedJobEntityType.Media,
                Culture = culture,
                JobType = EnterspeedJobType.Delete,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                ContentState = state,
            };
        }

        public EnterspeedJob GetDictionaryItemRootJob(string culture, EnterspeedContentState contentState)
        {
            return new EnterspeedJob
            {
                EntityId = UmbracoDictionariesRootEntity.EntityId,
                EntityType = EnterspeedJobEntityType.Dictionary,
                Culture = culture,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                JobType = EnterspeedJobType.Publish,
                State = EnterspeedJobState.Pending,
                ContentState = contentState
            };
        }

        public EnterspeedJob GetPublishMasterContentJob(string nodeId, string culture, EnterspeedContentState state)
        {
            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = nodeId,
                EntityType = EnterspeedJobEntityType.MasterContent,
                Culture = culture,
                JobType = EnterspeedJobType.Publish,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                ContentState = state
            };
        }

        public EnterspeedJob GetDeleteMasterContentJob(string nodeId, string culture, EnterspeedContentState state)
        {
            var now = DateTime.UtcNow;
            return new EnterspeedJob
            {
                EntityId = nodeId,
                EntityType = EnterspeedJobEntityType.MasterContent,
                Culture = culture,
                JobType = EnterspeedJobType.Delete,
                State = EnterspeedJobState.Pending,
                CreatedAt = now,
                UpdatedAt = now,
                ContentState = state
            };
        }
    }
}