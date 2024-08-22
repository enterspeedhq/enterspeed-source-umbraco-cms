using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Handlers
{
    public interface IEnterspeedPostJobsHandler
    {
        void Handle(IList<EnterspeedJob> processedJobs,
            IReadOnlyCollection<EnterspeedJob> existingFailedJobsToDelete,
            IList<EnterspeedJob> newFailedJobs);

        void HandleFailedJobs(IList<EnterspeedJob> newFailedJobs);
    }
}