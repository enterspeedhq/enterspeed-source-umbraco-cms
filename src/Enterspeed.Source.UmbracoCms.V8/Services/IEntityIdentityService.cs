using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEntityIdentityService
    {
        string GetId(IPublishedContent content, string culture);
        string GetId(int contentId, string culture);
    }
}
