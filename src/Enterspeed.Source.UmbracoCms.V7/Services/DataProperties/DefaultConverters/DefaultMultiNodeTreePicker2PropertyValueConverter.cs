using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Providers;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultMultiNodeTreePicker2PropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly EntityIdentityService _entityIdentityService;
        private readonly UmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultMultiNodeTreePicker2PropertyValueConverter()
        {
            _entityIdentityService = EnterspeedContext.Current.Services.EntityIdentityService;
            _mediaUrlProvider = EnterspeedContext.Current.Providers.MediaUrlProvider;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.MultiNodeTreePickerAlias + "2");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var arrayItems = new List<IEnterspeedProperty>();
            var nodeIds = new List<int>();

            var nodes = property.GetValue<IEnumerable<IPublishedContent>>();
            if (nodes != null)
            {
                nodeIds = nodes.Select(p => p.Id).ToList();
            }

            if (nodeIds.Any())
            {
                var umbracoHelper = UmbracoContextHelper.GetUmbracoHelper();
                foreach (var nodeId in nodeIds)
                {
                    var node = umbracoHelper.TypedContent(nodeId);

                    if (node == null)
                    {
                        node = umbracoHelper.TypedMember(nodeId);
                    }

                    if (node == null)
                    {
                        node = umbracoHelper.TypedMedia(nodeId);
                    }

                    var convertedNode = ConvertToEnterspeedProperty(node);
                    if (convertedNode != null)
                    {
                        arrayItems.Add(convertedNode);
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.PropertyTypeAlias, arrayItems.ToArray());
        }

        private ObjectEnterspeedProperty ConvertToEnterspeedProperty(IPublishedContent node)
        {
            if (node == null)
            {
                return null;
            }

            Dictionary<string, IEnterspeedProperty> properties;

            if (node.ItemType == PublishedItemType.Member)
            {
                properties = new Dictionary<string, IEnterspeedProperty>
                {
                    { "id", new NumberEnterspeedProperty(node.Id) },
                    { "name", new StringEnterspeedProperty(node.Name) },
                    { "memberType", new StringEnterspeedProperty(node.ContentType.Alias) }
                };
            }
            else
            {
                var url = node.Url;
                var id = node.Id.ToString();
                if (node.ItemType == PublishedItemType.Content)
                {
                    id = _entityIdentityService.GetId(node);
                    url = UmbracoContextHelper.GetUmbracoHelper().UrlAbsolute(node.Id);
                }
                else if (node.ItemType == PublishedItemType.Media)
                {
                    url = _mediaUrlProvider.GetUrl(node);
                }

                properties = new Dictionary<string, IEnterspeedProperty>
                {
                    { "id", new StringEnterspeedProperty(id) },
                    { "name", new StringEnterspeedProperty(node.Name) },
                    { "url", new StringEnterspeedProperty(url) }
                };
            }

            return new ObjectEnterspeedProperty(properties);
        }
    }
}