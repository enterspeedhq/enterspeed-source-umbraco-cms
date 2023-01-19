namespace Enterspeed.Source.UmbracoCms.Services
{
    public interface IUmbracoRichTextParser
    {
        string ParseInternalLink(string htmlValue);
        string PrefixRelativeImagesWithDomain(string html, string mediaDomain);
    }
}