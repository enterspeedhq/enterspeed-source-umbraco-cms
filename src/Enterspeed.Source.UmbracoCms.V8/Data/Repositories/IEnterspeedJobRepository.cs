using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V8.Data.Repositories
{
    public interface IEnterspeedJobRepository
    {
        IList<EnterspeedJob> GetFailedJobs();
        IList<EnterspeedJob> GetFailedJobs(List<string> entityIds);
        IList<EnterspeedJob> GetPendingJobs(int count);
        IList<EnterspeedJob> GetOldProcessingTasks(int olderThanMinutes = 60);
        void Save(List<EnterspeedJob> jobs);
        void Delete(List<int> ids);
    }
}
