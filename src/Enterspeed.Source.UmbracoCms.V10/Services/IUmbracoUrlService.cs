namespace Enterspeed.Source.UmbracoCms.V10.Services
{
    public interface IUmbracoUrlService
    {
        string GetUrlFromIdUrl(string idUrl, string culture);
        int GetIdFromIdUrl(string idUrl);
    }
}
