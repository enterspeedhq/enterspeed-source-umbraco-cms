using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.Models.Api;
using Enterspeed.Source.UmbracoCms.V14Plus.Models;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Services;

public interface IEnterspeedU14JobService
{
    SeedResponse Seed(bool publish, bool preview);
    Task<SeedResponse> CustomSeed(U14CustomSeedModel customSeedModel, bool publish, bool preview);
}