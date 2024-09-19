using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultCheckboxPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.TrueFalse");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            return new BooleanEnterspeedProperty(property.Alias, property.GetValue<bool>(culture));
        }
    }
}