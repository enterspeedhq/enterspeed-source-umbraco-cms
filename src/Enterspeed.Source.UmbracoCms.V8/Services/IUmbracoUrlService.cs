using Umbraco.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IUmbracoUrlService
    {
        string GetUrlFromIdUrl(string idUrl, string culture);
        int GetIdFromIdUrl(string idUrl);
        string GetMediaSrc(IMedia media);
    }
}
