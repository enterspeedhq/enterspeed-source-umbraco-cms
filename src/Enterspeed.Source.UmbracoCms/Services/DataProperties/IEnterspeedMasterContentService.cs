using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties
{
    public interface IEnterspeedMasterContentService
    {
        bool IsMasterContentEnabled();
        List<EnterspeedJob> CreatePublishMasterContentJobs(List<EnterspeedJob> variantJobs);
        List<EnterspeedJob> CreatePublishMasterContentJobs(string[] entityIds);
        List<EnterspeedJob> CreateDeleteMasterContentJobs(List<EnterspeedJob> variantJobs);
    }
}