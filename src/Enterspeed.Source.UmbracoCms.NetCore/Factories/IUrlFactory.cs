using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.NetCore.Factories
{
    public interface IUrlFactory
    {
        string GetUrl(IPublishedContent content, bool preview, string culture);
    }
}
