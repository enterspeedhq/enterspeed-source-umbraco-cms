using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V10.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V10.Handlers
{
    public interface IEnterspeedJobsHandler
    {
        void HandleJobs(IList<EnterspeedJob> jobs);
    }
}
