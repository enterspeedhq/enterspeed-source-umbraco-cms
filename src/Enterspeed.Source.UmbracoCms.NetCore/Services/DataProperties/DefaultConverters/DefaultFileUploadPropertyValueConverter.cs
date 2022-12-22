using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Extensions;
using Enterspeed.Source.UmbracoCms.Services.DataProperties;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Services.DataProperties.DefaultConverters
{
    public class DefaultFileUploadPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.UploadField");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<string>(culture);
            return new StringEnterspeedProperty(property.Alias, value);
        }
    }
}
