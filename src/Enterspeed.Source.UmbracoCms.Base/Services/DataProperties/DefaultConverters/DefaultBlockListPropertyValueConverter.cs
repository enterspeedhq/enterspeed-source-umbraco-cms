using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultBlockListPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultBlockListPropertyValueConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.BlockList");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue(culture);
            var dataPropertyService = _serviceProvider.GetRequiredService<IEnterspeedPropertyService>();

            switch (value)
            {
                case BlockListModel blockListModel:
                    return new ArrayEnterspeedProperty(property.Alias, blockListModel.Select(blockListItem => MapBlockListItem(blockListItem, culture, dataPropertyService)).ToArray());

                case BlockListItem blockListItem:
                    return MapBlockListItem(blockListItem, culture, dataPropertyService);

                default:
                    return new ArrayEnterspeedProperty(property.Alias, Array.Empty<IEnterspeedProperty>());
            }
        }

        protected IEnterspeedProperty MapBlockListItem(BlockListItem blockListItem, string culture, IEnterspeedPropertyService dataPropertyService)
        {
            var properties = new Dictionary<string, IEnterspeedProperty>();
            if (blockListItem.Content?.Properties != null)
            {
                var contentProperties = dataPropertyService.ConvertProperties(blockListItem.Content.Properties, culture);
                properties.Add("content", new ObjectEnterspeedProperty(contentProperties));
            }

            if (blockListItem.Settings?.Properties != null)
            {
                var settingsProperties = dataPropertyService.ConvertProperties(blockListItem.Settings.Properties, culture);
                properties.Add("settings", new ObjectEnterspeedProperty(settingsProperties));
            }

            if (blockListItem.Content?.ContentType != null)
            {
                properties.Add("contentType", new StringEnterspeedProperty(blockListItem.Content.ContentType.Alias));
            }

            return properties.Any()
                ? new ObjectEnterspeedProperty(properties)
                : null;
        }
    }
}
