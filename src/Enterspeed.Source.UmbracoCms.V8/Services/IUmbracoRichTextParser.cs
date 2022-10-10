namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IUmbracoRichTextParser
    {
        string ParseInternalLink(string htmlValue);
        string PrefixRelativeImagesWithDomain(string html, string mediaDomain);
    }
}