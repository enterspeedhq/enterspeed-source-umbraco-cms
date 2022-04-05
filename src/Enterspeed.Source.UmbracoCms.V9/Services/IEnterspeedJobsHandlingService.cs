using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using System.Collections.Generic;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public interface IEnterspeedJobsHandlingService
    {
        void HandleJobs(IList<EnterspeedJob> jobs);
        void HandlePendingJobs(int batchSize);
        void InvalidateOldProcessingJobs();
    }
}