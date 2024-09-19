using System;
using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultMultiNodeTreePickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IUmbracoMediaUrlProvider _mediaUrlProvider;
        private readonly ILogger<DefaultMultiNodeTreePickerPropertyValueConverter> _logger;

        public DefaultMultiNodeTreePickerPropertyValueConverter(
            IEntityIdentityService entityIdentityService,
            IUmbracoMediaUrlProvider mediaUrlProvider,
            ILogger<DefaultMultiNodeTreePickerPropertyValueConverter> logger)
        {
            _entityIdentityService = entityIdentityService;
            _mediaUrlProvider = mediaUrlProvider;
            _logger = logger;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.MultiNodeTreePicker");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var configuration = property.PropertyType.DataType.ConfigurationAs<MultiNodePickerConfiguration>();
            var isMultiple = configuration.MaxNumber != 1;
            var objectType = configuration?.TreeSource?.ObjectType;

            if (string.IsNullOrWhiteSpace(objectType))
            {
                throw new Exception($"Missing ObjectType/NodeType for MultiNodeTreePicker: {property.Alias}");
            }

            var arrayItems = new List<IEnterspeedProperty>();
            if (isMultiple)
            {
                var items = property.GetValue<IEnumerable<IPublishedContent>>(culture);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var contentObject = ConvertToEnterspeedProperty(item, culture, objectType);
                        if (contentObject != null)
                        {
                            arrayItems.Add(contentObject);
                        }
                    }
                }
            }
            else
            {
                var item = property.GetValue<IPublishedContent>(culture);
                var contentObject = ConvertToEnterspeedProperty(item, culture, objectType);
                if (contentObject != null)
                {
                    arrayItems.Add(contentObject);
                }
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
        }

        private ObjectEnterspeedProperty ConvertToEnterspeedProperty(IPublishedContent node, string culture, string objectType)
        {
            if (node == null)
            {
                return null;
            }

            if (!node.ContentType.VariesByCulture())
            {
                culture = null;
            }

            var id = node.Id.ToString();
            if (objectType.InvariantEquals("content"))
            {
                id = _entityIdentityService.GetId(node, culture);
            }

            var properties = new Dictionary<string, IEnterspeedProperty>
            {
                { "id", new StringEnterspeedProperty(id) },
                { "name", new StringEnterspeedProperty(node.Name) }
            };

            if (node.ContentType.ItemType is PublishedItemType.Content)
            {
                properties.Add("url", new StringEnterspeedProperty(node.GetUrl(_logger, culture, UrlMode.Absolute)));
            }
            else if (node.ContentType.ItemType is PublishedItemType.Media)
            {
                properties.Add("url", new StringEnterspeedProperty(_mediaUrlProvider.GetUrl(node)));
            }

            return new ObjectEnterspeedProperty(properties);
        }
    }
}
