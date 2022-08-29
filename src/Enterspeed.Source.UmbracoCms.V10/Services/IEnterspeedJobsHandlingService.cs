using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V10.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V10.Services
{
    public interface IEnterspeedJobsHandlingService
    {
        void HandleJobs(IList<EnterspeedJob> jobs);
        void HandlePendingJobs(int batchSize);
        void InvalidateOldProcessingJobs();
    }
}