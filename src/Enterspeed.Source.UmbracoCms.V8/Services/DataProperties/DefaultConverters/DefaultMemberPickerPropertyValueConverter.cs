using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultConverters
{
    public class DefaultMemberPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.MemberPicker");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<IPublishedContent>(culture);

            Dictionary<string, IEnterspeedProperty> properties = null;
            if (value != null)
            {
                properties = new Dictionary<string, IEnterspeedProperty>
                {
                    { "Id", new NumberEnterspeedProperty(value.Id) },
                    { "Name", new StringEnterspeedProperty(value.Name) },
                    { "MemberType", new StringEnterspeedProperty(value.ContentType.Alias) }
                };
            }

            return new ObjectEnterspeedProperty(property.Alias, properties);
        }
    }
}
