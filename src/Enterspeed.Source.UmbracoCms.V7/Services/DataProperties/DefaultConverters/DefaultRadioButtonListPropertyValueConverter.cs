using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultRadioButtonListPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.RadioButtonListAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out var prevalueId))
            {
                value = UmbracoContextHelper.GetUmbracoHelper().GetPreValueAsString(prevalueId);
            }
            else
            {
                value = null;
            }

            return new StringEnterspeedProperty(property.PropertyTypeAlias, value);
        }
    }
}
