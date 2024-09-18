using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Providers
{
    public interface IUmbracoMediaUrlProvider
    {
        string GetUrl(IPublishedContent media);
    }
}
