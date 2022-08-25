using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V10.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V10.Services.DataProperties.DefaultConverters
{
    public class DefaultTagsPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.Tags");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<IEnumerable<string>>(culture);
            var arrayItems = new List<IEnterspeedProperty>();
            foreach (var item in value)
            {
                arrayItems.Add(new StringEnterspeedProperty(item));
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
        }
    }
}
