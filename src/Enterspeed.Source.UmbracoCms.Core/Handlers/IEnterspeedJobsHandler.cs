using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Core.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Core.Handlers
{
    public interface IEnterspeedJobsHandler
    {
        void HandleJobs(IList<EnterspeedJob> jobsToProcess);
    }
}
