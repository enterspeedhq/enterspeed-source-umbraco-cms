using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Handlers
{
    public interface IEnterspeedPostJobsHandler
    {
        void Handle(IList<EnterspeedJob> processedJobs,
            IReadOnlyCollection<EnterspeedJob> existingFailedJobsToDelete,
            IList<EnterspeedJob> newFailedJobs);

        void HandleFailedJobs(IList<EnterspeedJob> newFailedJobs);
    }
}