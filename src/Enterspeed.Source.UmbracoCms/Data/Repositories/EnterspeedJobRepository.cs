using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Schemas;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Persistence;
#if NET5_0
using Umbraco.Cms.Core.Scoping;

#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.Data.Repositories
{
    public class EnterspeedJobRepository : IEnterspeedJobRepository
    {
        private readonly IScopeAccessor _scopeAccessor;
        private readonly IUmbracoMapper _mapper;

        public EnterspeedJobRepository(
            IUmbracoMapper mapper,
            IScopeAccessor scopeAccessor)
        {
            _mapper = mapper;
            _scopeAccessor = scopeAccessor;
        }

        private IUmbracoDatabase Database => _scopeAccessor.AmbientScope.Database;

        public IList<EnterspeedJob> GetFailedJobs()
        {
            var result = new List<EnterspeedJob>();
            var failedJobs = Database.Query<EnterspeedJobSchema>()
                .Where(x => x.JobState == EnterspeedJobState.Failed.GetHashCode())
                .OrderBy(x => x.CreatedAt)
                .ToList();

            result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(failedJobs));
            return result;
        }

        public IList<EnterspeedJob> GetFailedJobs(List<string> entityIds)
        {
            if (entityIds == null || !entityIds.Any())
            {
                return new List<EnterspeedJob>();
            }

            var result = new List<EnterspeedJob>();
            var failedJobs = Database.Query<EnterspeedJobSchema>()
                .Where(x => entityIds.Contains(x.EntityId) && x.JobState == EnterspeedJobState.Failed.GetHashCode())
                .OrderBy(x => x.CreatedAt)
                .ToList();

            result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(failedJobs));
            return result;
        }

        public IList<EnterspeedJob> GetPendingJobs(int count)
        {
            var result = new List<EnterspeedJob>();
            var pendingJobs = Database.Query<EnterspeedJobSchema>()
                .Where(x => x.JobState == EnterspeedJobState.Pending.GetHashCode())
                .OrderBy(x => x.CreatedAt)
                .Limit(count)
                .ToList();

            result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(pendingJobs));
            return result;
        }

        public int GetNumberOfPendingJobs()
        {
            var numberOfPendingJobs = Database.Query<EnterspeedJobSchema>()
                .Where(x => x.JobState == EnterspeedJobState.Pending.GetHashCode())
                .Count();

            return numberOfPendingJobs;
        }

        public IList<EnterspeedJob> GetOldProcessingTasks(int olderThanMinutes = 60)
        {
            var result = new List<EnterspeedJob>();
            var dateThreshhold = DateTime.UtcNow.AddMinutes(olderThanMinutes * -1);
            var pendingJobs = Database.Query<EnterspeedJobSchema>()
                .Where(x => x.JobState == EnterspeedJobState.Processing.GetHashCode() && x.UpdatedAt <= dateThreshhold)
                .ToList();
            result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(pendingJobs));
            return result;
        }

        public void Save(IList<EnterspeedJob> jobs)
        {
            if (jobs == null || !jobs.Any())
            {
                return;
            }

            foreach (var job in jobs)
            {
                var jobToSave = _mapper.Map<EnterspeedJobSchema>(job);
                Database.Save(jobToSave);
                job.Id = jobToSave.Id;
            }
        }

        public EnterspeedJob GetFailedJob(string entityId, string culture)
        {
            var schema = Database.Query<EnterspeedJobSchema>()
                .Where(x => x.EntityId.Contains(entityId)
                            && x.Culture.Equals(culture)
                            && x.JobState == EnterspeedJobState.Failed.GetHashCode())
                .FirstOrDefault();

            return _mapper.Map<EnterspeedJob>(schema);
        }

        public void Update(EnterspeedJob job)
        {
            var schema = _mapper.Map<EnterspeedJobSchema>(job);
            Database.Update(schema);
        }

        public void Delete(IList<int> ids)
        {
            Database.DeleteMany<EnterspeedJobSchema>()
                .Where(x => ids.Contains(x.Id))
                .Execute();
        }

        public void ClearPendingJobs()
        {
            Database.DeleteMany<EnterspeedJobSchema>()
                .Where(x => x.JobState == EnterspeedJobState.Pending.GetHashCode())
                .Execute();
        }
    }
}