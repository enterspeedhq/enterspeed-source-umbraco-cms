using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Core.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Core.Models
{
    public class EnterspeedJobHandlerResponse
    {
        public List<EnterspeedJob> FailedJobs { get; set; } = new List<EnterspeedJob>();
        public List<EnterspeedJob> FailedJobsToDelete { get; set; } = new List<EnterspeedJob>();
    }
}
