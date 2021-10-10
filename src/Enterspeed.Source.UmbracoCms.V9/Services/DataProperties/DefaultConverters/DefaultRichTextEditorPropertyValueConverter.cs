using System;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V9.Services.DataProperties.DefaultConverters
{
    public class DefaultRichTextEditorPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public DefaultRichTextEditorPropertyValueConverter(IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.TinyMCE");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<HtmlString>(culture).ToString();
            value = PrefixRelativeImagesWithDomain(value, _enterspeedConfigurationService.GetConfiguration().MediaDomain);
            return new StringEnterspeedProperty(property.Alias, value);
        }

        private string PrefixRelativeImagesWithDomain(string html, string mediaDomain)
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
                    imageNode.SetAttributeValue("src", new Uri(mediaDomainUrl, src).ToString());
                }
            }

            return htmlDocument.DocumentNode.InnerHtml;
        }
    }
}
