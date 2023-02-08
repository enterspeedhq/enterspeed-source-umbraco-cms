using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultContentPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly EntityIdentityService _entityIdentityService;

        public DefaultContentPickerPropertyValueConverter()
        {
            _entityIdentityService = EnterspeedContext.Current.Services.EntityIdentityService;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.ContentPickerAlias)
                   || propertyType.PropertyEditorAlias.Equals("Umbraco.ContentPicker2");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<string>();
            return new StringEnterspeedProperty(property.PropertyTypeAlias, _entityIdentityService.GetId(value));
        }
    }
}
