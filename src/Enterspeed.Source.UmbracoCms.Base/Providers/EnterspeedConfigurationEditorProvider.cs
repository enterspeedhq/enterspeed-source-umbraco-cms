using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    public class EnterspeedConfigurationEditorProvider : IEnterspeedConfigurationEditorProvider
    {
        public bool UseColorPickerLabel(IPublishedPropertyType propertyType)
        {
#if NET9_0_OR_GREATER
            return ConfigurationEditor.ConfigurationAs<ColorPickerConfiguration>(propertyType.DataType.ConfigurationObject).UseLabel;
#else
            return ConfigurationEditor.ConfigurationAs<ColorPickerConfiguration>(propertyType.DataType.Configuration).UseLabel;
#endif

        }
    }
}