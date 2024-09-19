using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Repositories
{
    public interface IEnterspeedJobRepository
    {
        IList<EnterspeedJob> GetFailedJobs();
        IList<EnterspeedJob> GetFailedJobs(List<string> entityIds);
        IList<EnterspeedJob> GetPendingJobs(int count);
        IList<EnterspeedJob> GetOldProcessingTasks(int olderThanMinutes = 60);
        EnterspeedJob GetFailedJob(string entityId, string culture);
        void Update(EnterspeedJob job);
        void Save(IList<EnterspeedJob> jobs);
        void Delete(IList<int> ids);
        void ClearPendingJobs();
        int GetNumberOfPendingJobs();
    }
}