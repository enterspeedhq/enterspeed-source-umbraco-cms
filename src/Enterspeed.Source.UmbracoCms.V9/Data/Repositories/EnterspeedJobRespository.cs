using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Schemas;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Scoping;

namespace Enterspeed.Source.UmbracoCms.V9.Data.Repositories
{
    public class EnterspeedJobRespository : IEnterspeedJobRepository
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IUmbracoMapper _mapper;

        public EnterspeedJobRespository(
            IScopeProvider scopeProvider,
            IUmbracoMapper mapper)
        {
            _scopeProvider = scopeProvider;
            _mapper = mapper;
        }

        public IList<EnterspeedJob> GetFailedJobs()
        {
            var result = new List<EnterspeedJob>();
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var failedJobs = scope.Database.Query<EnterspeedJobSchema>()
                    .Where(x => x.JobState == EnterspeedJobState.Failed.GetHashCode())
                    .OrderBy(x => x.CreatedAt)
                    .ToList();

                result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(failedJobs));
            }

            return result;
        }

        public IList<EnterspeedJob> GetFailedJobs(List<string> entityIds)
        {
            if (entityIds == null || !entityIds.Any())
            {
                return new List<EnterspeedJob>();
            }

            var result = new List<EnterspeedJob>();
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var failedJobs = scope.Database.Query<EnterspeedJobSchema>()
                    .Where(x => entityIds.Contains(x.EntityId) && x.JobState == EnterspeedJobState.Failed.GetHashCode())
                    .OrderBy(x => x.CreatedAt)
                    .ToList();

                result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(failedJobs));
            }

            return result;
        }

        public IList<EnterspeedJob> GetPendingJobs(int count)
        {
            var result = new List<EnterspeedJob>();
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var pendingJobs = scope.Database.Query<EnterspeedJobSchema>()
                    .Where(x => x.JobState == EnterspeedJobState.Pending.GetHashCode())
                    .OrderBy(x => x.CreatedAt)
                    .Limit(count)
                    .ToList();

                result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(pendingJobs));
            }

            return result;
        }

        public IList<EnterspeedJob> GetOldProcessingTasks(int olderThanMinutes = 60)
        {
            var result = new List<EnterspeedJob>();
            var dateThreshhold = DateTime.UtcNow.AddMinutes(olderThanMinutes * -1);
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var pendingJobs = scope.Database.Query<EnterspeedJobSchema>()
                    .Where(x => x.JobState == EnterspeedJobState.Processing.GetHashCode() && x.UpdatedAt <= dateThreshhold)
                    .ToList();

                result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(pendingJobs));
            }

            return result;
        }

        public void Save(List<EnterspeedJob> jobs)
        {
            if (jobs == null || !jobs.Any())
            {
                return;
            }

            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                foreach (var job in jobs)
                {
                    var jobToSave = _mapper.Map<EnterspeedJobSchema>(job);
                    scope.Database.Save(jobToSave);
                    job.Id = jobToSave.Id;
                }
            }
        }

        public void Delete(List<int> ids)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                scope.Database.DeleteMany<EnterspeedJobSchema>()
                    .Where(x => ids.Contains(x.Id))
                    .Execute();
            }
        }
    }
}
