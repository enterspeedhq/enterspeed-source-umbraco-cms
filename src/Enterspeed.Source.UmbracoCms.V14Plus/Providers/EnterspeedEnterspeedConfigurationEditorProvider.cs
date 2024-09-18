using Enterspeed.Source.UmbracoCms.Base.Providers;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Providers
{
    public class EnterspeedConfigurationEditorProvider : IEnterspeedConfigurationEditorProvider
    {
        public bool UseColorPickerLabel(IPublishedPropertyType propertyType)
        {
            return ConfigurationEditor.ConfigurationAs<ColorPickerConfiguration>(propertyType.DataType.ConfigurationObject).UseLabel;
        }
    }
}
