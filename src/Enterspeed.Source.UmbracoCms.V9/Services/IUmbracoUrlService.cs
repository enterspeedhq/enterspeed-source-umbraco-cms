namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public interface IUmbracoUrlService
    {
        string GetUrlFromIdUrl(string idUrl, string culture);
        int GetIdFromIdUrl(string idUrl);
    }
}
