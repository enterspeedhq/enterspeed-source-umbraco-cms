#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultBlockGridPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultBlockGridPropertyValueConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.BlockGrid");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<BlockGridModel>(culture);
            var blockGridProperties = new Dictionary<string, IEnterspeedProperty>();
            var arrayItems = new List<IEnterspeedProperty>();

            if (value != null)
            {
                // NOTE: This needs to be resolved manually, since it would cause a circular dependency if injected through constructor
                var dataPropertyService = _serviceProvider.GetRequiredService<IEnterspeedPropertyService>();

                if (value.GridColumns != null)
                {
                    blockGridProperties.Add("gridColumns", new NumberEnterspeedProperty("gridColumns", value.GridColumns.Value));
                }

                foreach (var item in value)
                {
                    if (item.Areas.Any())
                    {
                        var areas = AddAreas(item, dataPropertyService, culture);
                        arrayItems.Add(areas);
                    }
                    else
                    {
                        var properties = MapProperties(dataPropertyService, item, culture);
                        arrayItems.Add(new ObjectEnterspeedProperty(properties));
                    }
                }

                blockGridProperties.Add("items", new ArrayEnterspeedProperty("items", arrayItems.ToArray()));
            }

            return new ObjectEnterspeedProperty(property.Alias, blockGridProperties);
        }

        private static ObjectEnterspeedProperty AddAreas(BlockGridItem item, IEnterspeedPropertyService dataPropertyService, string culture)
        {
            var gridAreaProperties = new Dictionary<string, IEnterspeedProperty>();

            if (item.Areas.Any())
            {
                var arrayOfAreas = new List<IEnterspeedProperty>();
                gridAreaProperties = MapProperties(dataPropertyService, item, culture);

                foreach (var area in item.Areas)
                {
                    var arrayOfBlockGridItems = new List<IEnterspeedProperty>();

                    foreach (var blockGridItem in area)
                    {
                        var blockGridProperties = MapProperties(dataPropertyService, blockGridItem, culture);
                        if (blockGridItem.Areas.Any())
                        {
                            blockGridProperties.Add("areas", AddAreas(blockGridItem, dataPropertyService, culture));
                        }

                        arrayOfBlockGridItems.Add(new ObjectEnterspeedProperty(blockGridProperties));
                    }

                    var areaProperties = new Dictionary<string, IEnterspeedProperty>();
                    areaProperties.Add("alias", new StringEnterspeedProperty(area.Alias));
                    areaProperties.Add("items", new ArrayEnterspeedProperty("items", arrayOfBlockGridItems.ToArray()));

                    arrayOfAreas.Add(new ObjectEnterspeedProperty(areaProperties));
                }

                gridAreaProperties.Add("areas", new ArrayEnterspeedProperty("areas", arrayOfAreas.ToArray()));
            }

            return new ObjectEnterspeedProperty(gridAreaProperties);
        }

        private static Dictionary<string, IEnterspeedProperty> MapProperties(IEnterspeedPropertyService dataPropertyService, BlockGridItem item, string culture)
        {
            var properties = new Dictionary<string, IEnterspeedProperty>();

            if (item.Content?.Properties != null)
            {
                var contentProperties = dataPropertyService.ConvertProperties(item.Content.Properties, culture);
                properties.Add("content", new ObjectEnterspeedProperty(contentProperties));
            }

            if (item.Settings?.Properties != null)
            {
                var settingsProperties = dataPropertyService.ConvertProperties(item.Settings.Properties, culture);
                properties.Add("settings", new ObjectEnterspeedProperty(settingsProperties));
            }

            if (item.Content?.ContentType != null)
            {
                properties.Add("contentType", new StringEnterspeedProperty(item.Content.ContentType.Alias));
            }

            if (item.AreaGridColumns.HasValue)
            {
                properties.Add("areaGridColumns", new NumberEnterspeedProperty(item.AreaGridColumns.Value));
            }

            if (item.GridColumns.HasValue)
            {
                properties.Add("gridColumns", new NumberEnterspeedProperty(item.GridColumns.Value));
            }

            properties.Add("columnSpan", new NumberEnterspeedProperty(item.ColumnSpan));
            properties.Add("rowSpan", new NumberEnterspeedProperty(item.RowSpan));

            return properties;
        }
    }
}
#endif