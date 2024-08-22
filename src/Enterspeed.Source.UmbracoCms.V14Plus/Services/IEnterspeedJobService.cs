using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.Base.Models.Api;
using Enterspeed.Source.UmbracoCms.Models;

namespace Enterspeed.Source.UmbracoCms.Services;

public interface IEnterspeedJobService
{
    SeedResponse Seed(bool publish, bool preview);
    Task<SeedResponse> CustomSeed(CustomSeedModel customSeedModel, bool publish, bool preview);
}