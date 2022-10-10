namespace Enterspeed.Source.UmbracoCms.V10.Services;

public interface IUmbracoRichTextParser
{
    string ParseInternalLink(string htmlValue);
    string PrefixRelativeImagesWithDomain(string html, string mediaDomain);
}