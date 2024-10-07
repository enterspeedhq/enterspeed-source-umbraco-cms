using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultMemberPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.MemberPicker");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<IPublishedContent>(culture);

            Dictionary<string, IEnterspeedProperty> properties = null;
            if (value != null)
            {
                properties = new Dictionary<string, IEnterspeedProperty>
                {
                    { "id", new NumberEnterspeedProperty(value.Id) },
                    { "name", new StringEnterspeedProperty(value.Name) },
                    { "memberType", new StringEnterspeedProperty(value.ContentType.Alias) }
                };
            }

            return new ObjectEnterspeedProperty(property.Alias, properties);
        }
    }
}
