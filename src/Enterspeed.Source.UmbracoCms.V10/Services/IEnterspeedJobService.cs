using Enterspeed.Source.UmbracoCms.V10.Models.Api;

namespace Enterspeed.Source.UmbracoCms.V10.Services
{
    public interface IEnterspeedJobService
    {
        SeedResponse Seed(bool publish, bool preview);
    }
}
