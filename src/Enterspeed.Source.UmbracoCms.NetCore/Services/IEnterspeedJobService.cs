using Enterspeed.Source.UmbracoCms.NetCore.Models.Api;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services
{
    public interface IEnterspeedJobService
    {
        SeedResponse Seed(bool publish, bool preview);
    }
}
