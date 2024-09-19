using Enterspeed.Source.UmbracoCms.Base.Models.Api;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IEnterspeedJobService
    {
        SeedResponse Seed(bool publish, bool preview);
        SeedResponse CustomSeed(bool publish, bool preview, CustomSeed customSeed);
    }
}
