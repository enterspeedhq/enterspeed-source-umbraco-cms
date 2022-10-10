namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public interface IUmbracoRichTextParser
    {
        string ParseInternalLink(string htmlValue);
        string PrefixRelativeImagesWithDomain(string html, string mediaDomain);
    }
}