#if UMBRACO_18_OR_GREATER
using System;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    /// <summary>
    /// Handles the Umbraco.SingleBlock editor introduced in Umbraco 18. The v18 upgrade
    /// auto-migrates every single-mode Block List data type to this editor, so existing
    /// content stops matching the Umbraco.BlockList converter after upgrade. The value is
    /// a single BlockListItem, which the inherited Convert method already maps.
    /// </summary>
    public class DefaultSingleBlockPropertyValueConverter : DefaultBlockListPropertyValueConverter
    {
        public DefaultSingleBlockPropertyValueConverter(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.SingleBlock");
        }
    }
}
#endif
