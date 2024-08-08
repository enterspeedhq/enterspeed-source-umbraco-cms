using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Providers
{
    public interface IEnterspeedConfigurationEditorProvider
    {
        bool UseLabel(IPublishedPropertyType propertyType);
    }
}