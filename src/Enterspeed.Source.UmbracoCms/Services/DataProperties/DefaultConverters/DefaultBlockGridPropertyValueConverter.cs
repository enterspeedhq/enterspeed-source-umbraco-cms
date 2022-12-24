#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Services.DataProperties.DefaultConverters
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
            var arrayItems = new List<IEnterspeedProperty>();

            if (value != null)
            {
                // NOTE: This needs to be resolved manually, since it would cause a circular dependency if injected through constructor
                var dataPropertyService = _serviceProvider.GetRequiredService<IEnterspeedPropertyService>();

                foreach (var item in value)
                {
                    var properties = new Dictionary<string, IEnterspeedProperty>();

                    if (item.Areas.Any())
                    {
                        var areas = AddAreas(item, dataPropertyService);
                        properties.Add("areas", areas);
                    }
                    else
                    {
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
                    }

                    if (properties.Any())
                    {
                        arrayItems.Add(new ObjectEnterspeedProperty(properties));
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
        }

        private ArrayEnterspeedProperty AddAreas(BlockGridItem item, IEnterspeedPropertyService dataPropertyService)
        {
            var areasArrayItem = new List<IEnterspeedProperty>();

            if (item.Areas.Any())
            {
                foreach (var area in item.Areas)
                {
                    foreach (var blockGridItem in area)
                    {
                        var properties = new Dictionary<string, IEnterspeedProperty>();

                        var areaProperties = dataPropertyService.ConvertProperties(blockGridItem.Content.Properties);

                        if (areaProperties != null && areaProperties.Any())
                        {
                            properties.Add("content", new ObjectEnterspeedProperty(areaProperties));
                            properties.Add("contentType", new StringEnterspeedProperty(blockGridItem.Content.ContentType.Alias));
                        }

                        if (blockGridItem.Areas.Any())
                        {
                            properties.Add("areas", AddAreas(blockGridItem, dataPropertyService));
                        }

                        areasArrayItem.Add(new ObjectEnterspeedProperty(properties));
                    }
                }
            }

            return new ArrayEnterspeedProperty("areas", areasArrayItem.ToArray());
        }
    }
}

#endif