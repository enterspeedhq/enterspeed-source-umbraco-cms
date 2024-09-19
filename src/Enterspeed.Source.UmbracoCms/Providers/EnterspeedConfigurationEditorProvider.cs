using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    public class EnterspeedConfigurationEditorProvider : IEnterspeedConfigurationEditorProvider
    {
        public bool UseColorPickerLabel(IPublishedPropertyType propertyType)
        {
            return ConfigurationEditor.ConfigurationAs<ColorPickerConfiguration>(propertyType.DataType.Configuration).UseLabel;
        }
    }
}