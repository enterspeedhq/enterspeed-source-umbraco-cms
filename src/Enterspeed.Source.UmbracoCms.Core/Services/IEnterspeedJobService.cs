using Enterspeed.Source.UmbracoCms.Core.Models.Api;

namespace Enterspeed.Source.UmbracoCms.Core.Services
{
    public interface IEnterspeedJobService
    {
        SeedResponse Seed(bool publish, bool preview);
        SeedResponse CustomSeed(bool publish, bool preview, CustomSeed customSeed);
    }
}
