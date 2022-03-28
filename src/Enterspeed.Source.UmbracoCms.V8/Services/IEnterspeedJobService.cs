using Enterspeed.Source.UmbracoCms.V8.Models.Api;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEnterspeedJobService
    {
        SeedResponse Seed(bool publish, bool preview);
    }
}
