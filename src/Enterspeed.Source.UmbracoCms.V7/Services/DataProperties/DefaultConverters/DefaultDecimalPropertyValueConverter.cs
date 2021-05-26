using System.Globalization;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultDecimalPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.DecimalAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<decimal>();
            var number = 0d;

            if (double.TryParse(value.ToString(CultureInfo.InvariantCulture), out var n))
            {
                number = n;
            }

            return new NumberEnterspeedProperty(property.PropertyTypeAlias, number);
        }
    }
}
