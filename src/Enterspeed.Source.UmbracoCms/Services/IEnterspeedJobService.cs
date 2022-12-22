using Enterspeed.Source.UmbracoCms.Models.Api;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public interface IEnterspeedJobService
    {
        SeedResponse Seed(bool publish, bool preview);
    }
}
