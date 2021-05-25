using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultNumericPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.IntegerAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<int>();
            return new NumberEnterspeedProperty(property.PropertyTypeAlias, value);
        }
    }
}
