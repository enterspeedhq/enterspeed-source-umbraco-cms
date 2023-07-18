using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Umbraco.Web.Templates;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class UmbracoRichTextParser : IUmbracoRichTextParser
    {
        private readonly HtmlLocalLinkParser _htmlLocalLinkParser;
        private readonly HtmlImageSourceParser _htmlImageSourceParser;

        public UmbracoRichTextParser(HtmlLocalLinkParser htmlLocalLinkParser, HtmlImageSourceParser htmlImageSourceParser)
        {
            _htmlLocalLinkParser = htmlLocalLinkParser;
            _htmlImageSourceParser = htmlImageSourceParser;
        }

        public string ParseInternalLink(string htmlValue)
        {
            var parsedHtmlValue = _htmlLocalLinkParser.EnsureInternalLinks(htmlValue);
            parsedHtmlValue = _htmlImageSourceParser.EnsureImageSources(parsedHtmlValue);

            return parsedHtmlValue;
        }

        public string PrefixRelativeImagesWithDomain(string html, string mediaDomain)
        {
            if (string.IsNullOrWhiteSpace(html) || string.IsNullOrWhiteSpace(mediaDomain))
            {
                return html;
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var imageNodes = htmlDocument.DocumentNode.SelectNodes("//img");
            if (imageNodes == null || !imageNodes.Any())
            {
                return html;
            }

            var mediaDomainUrl = new Uri(mediaDomain);
            foreach (var imageNode in imageNodes)
            {
                var src = imageNode.GetAttributeValue("src", string.Empty);
                if (src.StartsWith("/media/"))
                {
                    src = $"{mediaDomainUrl.AbsoluteUri.TrimEnd('/')}/{src.TrimStart('/')}";
                    imageNode.SetAttributeValue("src", src);
                }
            }

            return htmlDocument.DocumentNode.InnerHtml;
        }
    }
}