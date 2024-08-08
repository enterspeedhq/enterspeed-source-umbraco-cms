using Enterspeed.Source.UmbracoCms.Providers;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Enterspeed.Source.UmbracoCms.V14.Providers
{
    public class EnterspeedConfigurationEditorProvider : IEnterspeedConfigurationEditorProvider
    {
        public bool UseColorPickerLabel(IPublishedPropertyType propertyType)
        {
            return ConfigurationEditor.ConfigurationAs<ColorPickerConfiguration>(propertyType.DataType.ConfigurationObject).UseLabel;
        }
    }
}
