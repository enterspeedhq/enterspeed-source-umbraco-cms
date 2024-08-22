using System.Collections.Generic;
using System.Globalization;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultSliderPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.Slider");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var configuration = property.PropertyType.DataType.ConfigurationAs<SliderConfiguration>();
            if (configuration.EnableRange)
            {
                var range = property.GetValue<Range<decimal>>(culture);
                var properties = new Dictionary<string, IEnterspeedProperty>();
                properties.Add("minimum", new NumberEnterspeedProperty(ConvertToDouble(range.Minimum)));
                properties.Add("maximum", new NumberEnterspeedProperty(ConvertToDouble(range.Maximum)));

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
