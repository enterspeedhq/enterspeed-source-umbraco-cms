using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V9.Models
{
    public class EnterspeedJobHandlerResponse
    {
        public List<EnterspeedJob> FailedJobs { get; set; } = new List<EnterspeedJob>();
        public List<EnterspeedJob> FailedJobsToDelete { get; set; } = new List<EnterspeedJob>();
    }
}
