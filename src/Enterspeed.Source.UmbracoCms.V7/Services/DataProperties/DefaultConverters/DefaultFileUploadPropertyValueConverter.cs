using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Providers;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultFileUploadPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly UmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultFileUploadPropertyValueConverter()
        {
            _mediaUrlProvider = EnterspeedContext.Current.Providers.MediaUrlProvider;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.UploadFieldAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<string>();
            value = _mediaUrlProvider.GetUrl(value);
            return new StringEnterspeedProperty(property.PropertyTypeAlias, value);
        }
    }
}
