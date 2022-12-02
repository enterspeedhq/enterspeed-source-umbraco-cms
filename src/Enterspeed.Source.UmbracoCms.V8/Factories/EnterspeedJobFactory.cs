using System;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Factories
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
    }
}
