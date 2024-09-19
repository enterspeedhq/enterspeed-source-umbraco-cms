namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IUmbracoRichTextParser
    {
        string ParseInternalLink(string htmlValue);
        string PrefixRelativeImagesWithDomain(string html, string mediaDomain);
    }
}