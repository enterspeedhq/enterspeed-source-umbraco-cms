using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Data.Repositories
{
    public interface IEnterspeedJobRepository
    {
        IList<EnterspeedJob> GetFailedJobs();
        IList<EnterspeedJob> GetFailedJobs(List<string> entityIds);
        IList<EnterspeedJob> GetPendingJobs(int count);
        IList<EnterspeedJob> GetOldProcessingTasks(int olderThanMinutes = 60);
        void Save(IList<EnterspeedJob> jobs);
        void Delete(IList<int> ids);
    }
}
