using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties
{
    public interface IEnterspeedPropertyValueConverter
    {
        bool IsConverter(PublishedPropertyType propertyType);
        IEnterspeedProperty Convert(IPublishedProperty property);
    }
}
