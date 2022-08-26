using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V10.Providers
{
    public interface IUmbracoMediaUrlProvider
    {
        string GetUrl(IPublishedContent media);
    }
}
