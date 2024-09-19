using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Schemas;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Persistence;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif


namespace Enterspeed.Source.UmbracoCms.Base.Data.Repositories
{
    public class EnterspeedJobRepository : IEnterspeedJobRepository
    {
        private readonly IScopeAccessor _scopeAccessor;
        private readonly IUmbracoMapper _mapper;
        private readonly IScopeProvider _scopeProvider;

        public EnterspeedJobRepository(
            IUmbracoMapper mapper,
            IScopeAccessor scopeAccessor,
            IScopeProvider scopeProvider)
        {
            _mapper = mapper;
            _scopeAccessor = scopeAccessor;
            _scopeProvider = scopeProvider;
        }

        private IUmbracoDatabase Database => _scopeAccessor.AmbientScope.Database;

        public IList<EnterspeedJob> GetFailedJobs()
        {
            using (_scopeProvider.CreateScope(autoComplete: true))
            {
                var result = new List<EnterspeedJob>();
                var failedJobs = Database.Query<EnterspeedJobSchema>()
                    .Where(x => x.JobState == EnterspeedJobState.Failed.GetHashCode())
                    .OrderBy(x => x.CreatedAt)
                    .ToList();

                result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(failedJobs));
                return result;
            }
        }

        public IList<EnterspeedJob> GetFailedJobs(List<string> entityIds)
        {
            if (entityIds == null || !entityIds.Any())
            {
                return new List<EnterspeedJob>();
            }

            using (_scopeProvider.CreateScope(autoComplete: true))
            {
                var result = new List<EnterspeedJob>();
                var failedJobs = Database.Query<EnterspeedJobSchema>()
                    .Where(x => entityIds.Contains(x.EntityId) && x.JobState == EnterspeedJobState.Failed.GetHashCode())
                    .OrderBy(x => x.CreatedAt)
                    .ToList();

                result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(failedJobs));
                return result;
            }
        }

        public IList<EnterspeedJob> GetPendingJobs(int count)
        {
            var result = new List<EnterspeedJob>();

            using (_scopeProvider.CreateScope(autoComplete: true))
            {
                var pendingJobs = Database.Query<EnterspeedJobSchema>()
                    .Where(x => x.JobState == EnterspeedJobState.Pending.GetHashCode())
                    .OrderBy(x => x.CreatedAt)
                    .Limit(count)
                    .ToList();

                result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(pendingJobs));
                return result;
            }
        }

        public int GetNumberOfPendingJobs()
        {
            using (_scopeProvider.CreateScope(autoComplete: true))
            {
                var numberOfPendingJobs = Database.Query<EnterspeedJobSchema>()
                    .Where(x => x.JobState == EnterspeedJobState.Pending.GetHashCode())
                    .Count();

                return numberOfPendingJobs;
            }
        }

        public IList<EnterspeedJob> GetOldProcessingTasks(int olderThanMinutes = 60)
        {
            using (_scopeProvider.CreateScope(autoComplete: true))
            {
                var result = new List<EnterspeedJob>();
                var dateTresHold = DateTime.UtcNow.AddMinutes(olderThanMinutes * -1);
                var pendingJobs = Database.Query<EnterspeedJobSchema>()
                    .Where(x => x.JobState == EnterspeedJobState.Processing.GetHashCode() &&
                                x.UpdatedAt <= dateTresHold)
                    .ToList();
                result.AddRange(_mapper.MapEnumerable<EnterspeedJobSchema, EnterspeedJob>(pendingJobs));
                return result;
            }
        }

        public void Save(IList<EnterspeedJob> jobs)
        {
            if (jobs == null || !jobs.Any())
            {
                return;
            }

            using (var scope = _scopeProvider.CreateScope())
            {
                foreach (var job in jobs)
                {
                    var jobToSave = _mapper.Map<EnterspeedJobSchema>(job);
                    Database.Save(jobToSave);
                    job.Id = jobToSave.Id;
                }

                scope.Complete();
            }
        }

        public EnterspeedJob GetFailedJob(string entityId, string culture)
        {
            using (_scopeProvider.CreateScope(autoComplete: true))
            {
                var schema = Database.Query<EnterspeedJobSchema>()
                    .Where(x => x.EntityId.Contains(entityId)
                                && x.Culture.Equals(culture)
                                && x.JobState == EnterspeedJobState.Failed.GetHashCode())
                    .FirstOrDefault();

                return _mapper.Map<EnterspeedJob>(schema);
            }
        }

        public void Update(EnterspeedJob job)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var schema = _mapper.Map<EnterspeedJobSchema>(job);
                Database.Update(schema);
                scope.Complete();
            }
        }

        public void Delete(IList<int> ids)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                Database.DeleteMany<EnterspeedJobSchema>()
                    .Where(x => ids.Contains(x.Id))
                    .Execute();
                scope.Complete();
            }
        }

        public void ClearPendingJobs()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                Database.DeleteMany<EnterspeedJobSchema>()
                    .Where(x => x.JobState == EnterspeedJobState.Pending.GetHashCode())
                    .Execute();
                scope.Complete();
            }
        }
    }
}