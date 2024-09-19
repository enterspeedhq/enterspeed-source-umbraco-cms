using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IEnterspeedJobsHandlingService
    {
        bool IsJobsProcessingEnabled();
        void HandleJobs(IList<EnterspeedJob> jobs);
        void HandlePendingJobs(int batchSize);
        void InvalidateOldProcessingJobs();
    }
}