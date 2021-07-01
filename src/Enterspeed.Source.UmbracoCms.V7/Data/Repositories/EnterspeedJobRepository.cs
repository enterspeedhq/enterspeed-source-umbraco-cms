using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Enterspeed.Source.UmbracoCms.V7.Data.Schemas;
using Umbraco.Core;

namespace Enterspeed.Source.UmbracoCms.V7.Data.Repositories
{
    public class EnterspeedJobRepository
    {
        private readonly string _tableName = "EnterspeedJobs";
        public IList<EnterspeedJob> GetFailedJobs()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var failedJobs = db.Fetch<EnterspeedJobSchema>(
                $"SELECT * FROM [{_tableName}] WHERE [JobState] = {EnterspeedJobState.Failed.GetHashCode()} ORDER BY [CreatedAt]");

            var result = failedJobs?.Select(MapFromSchema).Where(x => x != null).ToList();

            return result ?? new List<EnterspeedJob>();
        }

        public IList<EnterspeedJob> GetFailedJobs(List<string> entityIds)
        {
            if (entityIds == null || !entityIds.Any())
            {
                return new List<EnterspeedJob>();
            }

            var db = ApplicationContext.Current.DatabaseContext.Database;

            var failedJobs = db.Fetch<EnterspeedJobSchema>(
                $"SELECT * FROM [{_tableName}] WHERE [EntityId] IN ({string.Join(",", entityIds.Select(x => $"'{x}'"))}) AND [JobState] = {EnterspeedJobState.Failed.GetHashCode()} ORDER BY [CreatedAt]");

            var result = failedJobs?.Select(MapFromSchema).Where(x => x != null).ToList();

            return result ?? new List<EnterspeedJob>();
        }

        public IList<EnterspeedJob> GetPendingJobs(int count)
        {
            if (count <= 0)
            {
                return new List<EnterspeedJob>();
            }

            var db = ApplicationContext.Current.DatabaseContext.Database;

            var pendingJobs = db.Fetch<EnterspeedJobSchema>(
                $"SELECT TOP {count} * FROM [{_tableName}] WHERE [JobState] = {EnterspeedJobState.Pending.GetHashCode()} ORDER BY [CreatedAt]");

            var result = pendingJobs?.Select(MapFromSchema).Where(x => x != null).ToList();

            return result ?? new List<EnterspeedJob>();
        }

        public IList<EnterspeedJob> GetOldProcessingTasks(int olderThanMinutes = 60)
        {
            var dateThreshhold = DateTime.UtcNow.AddMinutes(olderThanMinutes * -1).ToString("yyyy-MM-dd HH\\:mm\\:ss");
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var pendingJobs = db.Fetch<EnterspeedJobSchema>(
                $"SELECT * FROM [{_tableName}] WHERE [JobState] = {EnterspeedJobState.Processing.GetHashCode()} AND [UpdatedAt] <= '{dateThreshhold}.000'");

            var result = pendingJobs?.Select(MapFromSchema).Where(x => x != null).ToList();

            return result ?? new List<EnterspeedJob>();
        }

        public void Save(List<EnterspeedJob> jobs)
        {
            if (jobs == null || !jobs.Any())
            {
                return;
            }

            var db = ApplicationContext.Current.DatabaseContext.Database;
            foreach (var job in jobs)
            {
                var jobToSave = MapToSchema(job);
                db.Save(jobToSave);
                job.Id = jobToSave.Id;
            }
        }

        public void Delete(List<int> ids)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            db.Execute($"DELETE FROM [{_tableName}] WHERE [Id] IN ({string.Join(",", ids)})");
        }

        private EnterspeedJob MapFromSchema(EnterspeedJobSchema job)
        {
            if (job == null)
            {
                return null;
            }

            return new EnterspeedJob
            {
                Id = job.Id,
                EntityId = job.EntityId,
                CreatedAt = job.CreatedAt,
                Culture = job.Culture,
                Exception = job.Exception,
                JobType = (EnterspeedJobType)job.JobType,
                State = (EnterspeedJobState)job.JobState,
                UpdatedAt = job.UpdatedAt,
                EntityType = (EnterspeedJobEntityType)job.EntityType
            };
        }

        private EnterspeedJobSchema MapToSchema(EnterspeedJob job)
        {
            if (job == null)
            {
                return null;
            }

            return new EnterspeedJobSchema
            {
                Id = job.Id,
                EntityId = job.EntityId,
                CreatedAt = job.CreatedAt,
                Culture = job.Culture,
                Exception = job.Exception,
                JobType = job.JobType.GetHashCode(),
                JobState = job.State.GetHashCode(),
                UpdatedAt = job.UpdatedAt,
                EntityType = job.EntityType.GetHashCode()
            };
        }
    }
}
