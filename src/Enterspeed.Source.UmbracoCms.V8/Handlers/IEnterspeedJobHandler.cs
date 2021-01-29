using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers
{
    public interface IEnterspeedJobHandler
    {
        void HandlePendingJobs(int batchSize);
        void HandleJobs(List<EnterspeedJob> jobs);
        void InvalidateOldProcessingJobs();
    }
}
