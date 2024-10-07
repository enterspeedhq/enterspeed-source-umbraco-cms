using System;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultDateTimePropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.DateTime");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var date = property.GetValue<DateTime>(culture);
            return new StringEnterspeedProperty(property.Alias, date.ToEnterspeedFormatString());
        }
    }
}
