using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers
{
    public interface IEnterspeedJobsHandler
    {
        void HandleJobs(IList<EnterspeedJob> jobs);
    }
}
