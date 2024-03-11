using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Core.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Core.Services.DataProperties
{
    public interface IEnterspeedMasterContentService
    {
        bool IsMasterContentEnabled();
        List<EnterspeedJob> CreatePublishMasterContentJobs(List<EnterspeedJob> variantJobs);
        List<EnterspeedJob> CreatePublishMasterContentJobs(string[] entityIds);
        List<EnterspeedJob> CreateDeleteMasterContentJobs(List<EnterspeedJob> variantJobs);
    }
}