using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Extensions;
using Enterspeed.Source.UmbracoCms.Services.DataProperties;
using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Services.DataProperties.DefaultConverters
{
    public class DefaultMarkdownEditorPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.MarkdownEditor");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<HtmlString>(culture);
            return new StringEnterspeedProperty(property.Alias, value.ToString());
        }
    }
}
