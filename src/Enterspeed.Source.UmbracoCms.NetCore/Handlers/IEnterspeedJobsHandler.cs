using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.NetCore.Data.Models;

namespace Enterspeed.Source.UmbracoCms.NetCore.Handlers
{
    public interface IEnterspeedJobsHandler
    {
        void HandleJobs(IList<EnterspeedJob> jobs);
    }
}
