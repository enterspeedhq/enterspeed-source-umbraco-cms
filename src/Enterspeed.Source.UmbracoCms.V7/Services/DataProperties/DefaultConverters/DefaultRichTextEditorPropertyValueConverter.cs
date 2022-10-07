using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Providers;
using HtmlAgilityPack;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultRichTextEditorPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly UmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultRichTextEditorPropertyValueConverter()
        {
            _mediaUrlProvider = EnterspeedContext.Current.Providers.MediaUrlProvider;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.TinyMCEAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<string>();
            value = PrefixRelativeImagesWithDomain(value);
            return new StringEnterspeedProperty(property.PropertyTypeAlias, value);
        }

        private string PrefixRelativeImagesWithDomain(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
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

            foreach (var imageNode in imageNodes)
            {
                var src = imageNode.GetAttributeValue("src", string.Empty);
                src = _mediaUrlProvider.GetUrl(src);
                imageNode.SetAttributeValue("src", src);
            }

            return htmlDocument.DocumentNode.InnerHtml;
        }
    }
}
