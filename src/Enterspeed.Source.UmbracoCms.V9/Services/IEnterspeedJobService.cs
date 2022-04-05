using Enterspeed.Source.UmbracoCms.V9.Models.Api;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public interface IEnterspeedJobService
    {
        SeedResponse Seed(bool publish, bool preview);
    }
}
