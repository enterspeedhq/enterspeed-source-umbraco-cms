using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V9.Providers
{
    public interface IUmbracoMediaUrlProvider
    {
        string GetUrl(IPublishedContent media);
    }
}
