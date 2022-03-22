using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
{
    public interface IEnterspeedJobsHandler
    {
        void HandleJobs(IList<EnterspeedJob> jobs);
    }
}
