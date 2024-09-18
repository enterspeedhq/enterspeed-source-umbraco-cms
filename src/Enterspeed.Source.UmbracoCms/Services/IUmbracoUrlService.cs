namespace Enterspeed.Source.UmbracoCms.Services
{
    public interface IUmbracoUrlService
    {
        string GetUrlFromIdUrl(string idUrl, string culture);
        int GetIdFromIdUrl(string idUrl);
    }
}
