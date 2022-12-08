using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.NetCore.Data.Models;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services
{
    public interface IEnterspeedJobsHandlingService
    {
        void HandleJobs(IList<EnterspeedJob> jobs);
        void HandlePendingJobs(int batchSize);
        void InvalidateOldProcessingJobs();
    }
}