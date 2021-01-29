using System.Collections.Generic;
using System.Globalization;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultConverters
{
    public class DefaultSliderPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.Slider");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var configuration = property.PropertyType.DataType.ConfigurationAs<SliderConfiguration>();
            if (configuration.EnableRange)
            {
                var range = property.GetValue<Range<decimal>>(culture);
                var properties = new Dictionary<string, IEnterspeedProperty>();
                properties.Add("Minimum", new NumberEnterspeedProperty(ConvertToDouble(range.Minimum)));
                properties.Add("Maximum", new NumberEnterspeedProperty(ConvertToDouble(range.Maximum)));

                return new ObjectEnterspeedProperty(property.Alias, properties);
            }

            var value = property.GetValue<decimal>(culture);
            return new NumberEnterspeedProperty(property.Alias, ConvertToDouble(value));
        }

        private double ConvertToDouble(decimal d)
        {
            return double.Parse(d.ToString(CultureInfo.InvariantCulture));
        }
    }
}
