using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties
{
    public interface IEnterspeedPropertyValueConverter
    {
        bool IsConverter(IPublishedPropertyType propertyType);
        IEnterspeedProperty Convert(IPublishedProperty property, string culture);
    }
}
