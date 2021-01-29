using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultConverters
{
    public class DefaultContentPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IEntityIdentityService _entityIdentityService;

        public DefaultContentPickerPropertyValueConverter(
            IEntityIdentityService entityIdentityService)
        {
            _entityIdentityService = entityIdentityService;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.ContentPicker");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<IPublishedContent>(culture);
            var contentId = _entityIdentityService.GetId(value, culture);
            return new StringEnterspeedProperty(property.Alias, contentId);
        }
    }
}
