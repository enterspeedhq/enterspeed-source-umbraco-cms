namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IUmbracoUrlService
    {
        string GetUrlFromIdUrl(string idUrl, string culture);
        int GetIdFromIdUrl(string idUrl);
    }
}
