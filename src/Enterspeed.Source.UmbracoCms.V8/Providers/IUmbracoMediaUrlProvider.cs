using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Providers
{
    public interface IUmbracoMediaUrlProvider
    {
        string GetUrl(IPublishedContent media);
    }
}
