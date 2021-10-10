using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
{
    public interface IEnterspeedJobHandler
    {
        void HandlePendingJobs(int batchSize);
        void HandleJobs(List<EnterspeedJob> jobs);
        void InvalidateOldProcessingJobs();
    }
}
