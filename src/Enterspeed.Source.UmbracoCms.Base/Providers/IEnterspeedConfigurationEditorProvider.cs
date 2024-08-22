using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    public interface IEnterspeedConfigurationEditorProvider
    {
        bool UseColorPickerLabel(IPublishedPropertyType propertyType);
    }
}