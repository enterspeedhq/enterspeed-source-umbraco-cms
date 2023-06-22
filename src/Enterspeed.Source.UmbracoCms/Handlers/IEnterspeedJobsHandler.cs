using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Handlers
{
    public interface IEnterspeedJobsHandler
    {
        void HandleJobs(IList<EnterspeedJob> jobsToProcess);
    }
}
