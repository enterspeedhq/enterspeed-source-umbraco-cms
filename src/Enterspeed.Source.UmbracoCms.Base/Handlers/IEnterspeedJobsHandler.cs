using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Handlers
{
    public interface IEnterspeedJobsHandler
    {
        void HandleJobs(IList<EnterspeedJob> jobsToProcess);
    }
}
