using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Services.DataProperties
{
    public interface IEnterspeedMasterContentService
    {
        bool IsMasterContentEnabled();
        List<EnterspeedJob> CreateMasterContentJobs(List<EnterspeedJob> variantJobs);
    }
}