using System.Web;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultConverters
{
    public class DefaultRichTextEditorPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IUmbracoRichTextParser _umbracoRichTextParser;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public DefaultRichTextEditorPropertyValueConverter(IUmbracoRichTextParser umbracoRichTextParser, IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _umbracoRichTextParser = umbracoRichTextParser;
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.TinyMCE");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<HtmlString>(culture).ToString();
            value = _umbracoRichTextParser.PrefixRelativeImagesWithDomain(value, _enterspeedConfigurationService.GetConfiguration().MediaDomain);
            return new StringEnterspeedProperty(property.Alias, value);
        }
    }
}
