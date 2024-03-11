using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Core.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Core.Services
{
    public interface IEnterspeedJobsHandlingService
    {
        bool IsJobsProcessingEnabled();
        void HandleJobs(IList<EnterspeedJob> jobs);
        void HandlePendingJobs(int batchSize);
        void InvalidateOldProcessingJobs();
    }
}