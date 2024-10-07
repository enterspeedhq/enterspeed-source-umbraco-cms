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
            var value = property.GetValue<BlockListModel>(culture);
            var arrayItems = new List<IEnterspeedProperty>();

            if (value != null)
            {
                // NOTE: This needs to be resolved manually, since it would cause a circular dependency if injected through constructor
                var dataPropertyService = _serviceProvider.GetRequiredService<IEnterspeedPropertyService>();

                foreach (var item in value)
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

                    if (properties.Any())
                    {
                        arrayItems.Add(new ObjectEnterspeedProperty(properties));
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
        }
    }
}
