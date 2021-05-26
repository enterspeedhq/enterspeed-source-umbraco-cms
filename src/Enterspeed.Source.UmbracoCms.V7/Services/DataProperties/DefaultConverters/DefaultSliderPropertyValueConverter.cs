using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultSliderPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.SliderAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var values = property.GetValue<string>()?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList();

            if (values == null || !values.Any())
            {
                return new NumberEnterspeedProperty(property.PropertyTypeAlias, 0);
            }

            var isRange = values.Count > 1;
            if (isRange)
            {
                var properties = new Dictionary<string, IEnterspeedProperty>();
                properties.Add("minimum", new NumberEnterspeedProperty(ConvertToDouble(values.First())));
                properties.Add("maximum", new NumberEnterspeedProperty(ConvertToDouble(values.Last())));

                return new ObjectEnterspeedProperty(property.PropertyTypeAlias, properties);
            }

            return new NumberEnterspeedProperty(property.PropertyTypeAlias, ConvertToDouble(values.First()));
        }

        private double ConvertToDouble(decimal d)
        {
            return double.Parse(d.ToString(CultureInfo.InvariantCulture));
        }
    }
}
