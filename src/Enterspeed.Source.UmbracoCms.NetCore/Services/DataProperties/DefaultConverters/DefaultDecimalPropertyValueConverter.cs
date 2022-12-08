using System.Globalization;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.NetCore.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services.DataProperties.DefaultConverters
{
    public class DefaultDecimalPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.Decimal");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<decimal>(culture);
            var number = 0d;

            if (double.TryParse(value.ToString(CultureInfo.InvariantCulture), out var n))
            {
                number = n;
            }

            return new NumberEnterspeedProperty(property.Alias, number);
        }
    }
}
