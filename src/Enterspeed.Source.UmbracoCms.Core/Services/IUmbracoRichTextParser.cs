namespace Enterspeed.Source.UmbracoCms.Core.Services
{
    public interface IUmbracoRichTextParser
    {
        string ParseInternalLink(string htmlValue);
        string PrefixRelativeImagesWithDomain(string html, string mediaDomain);
    }
}