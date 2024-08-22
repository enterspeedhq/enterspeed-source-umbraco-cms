using System;
using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultMultiUrlPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IUmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultMultiUrlPickerPropertyValueConverter(
            IUmbracoContextFactory umbracoContextFactory,
            IEntityIdentityService entityIdentityService,
            IUmbracoMediaUrlProvider mediaUrlProvider)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _entityIdentityService = entityIdentityService;
            _mediaUrlProvider = mediaUrlProvider;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.MultiUrlPicker");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var isMultiple = property.PropertyType.DataType.ConfigurationAs<MultiUrlPickerConfiguration>().MaxNumber != 1;
            var arrayItems = new List<IEnterspeedProperty>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                if (isMultiple)
                {
                    var items = property.GetValue<IEnumerable<Link>>(culture);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            var linkObject = ConvertToEnterspeedProperty(item, context.UmbracoContext, culture);
                            if (linkObject != null)
                            {
                                arrayItems.Add(linkObject);
                            }
                        }
                    }
                }
                else
                {
                    var item = property.GetValue<Link>(culture);
                    var linkObject = ConvertToEnterspeedProperty(item, context.UmbracoContext, culture);
                    if (linkObject != null)
                    {
                        arrayItems.Add(linkObject);
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
        }

        private ObjectEnterspeedProperty ConvertToEnterspeedProperty(Link link, IUmbracoContext context, string culture)
        {
            if (link == null)
            {
                return null;
            }

            var properties = new Dictionary<string, IEnterspeedProperty>
            {
                { "name", new StringEnterspeedProperty(link.Name) },
                { "target", new StringEnterspeedProperty(link.Target) }
            };

            var linkType = Enum.GetName(typeof(LinkType), link.Type);
            properties.Add("type", new StringEnterspeedProperty(linkType));
            var url = link.Url;

            var idProperty = new StringEnterspeedProperty(string.Empty);
            if (link.Udi != null)
            {
                if (link.Udi.EntityType == "document")
                {
                    var content = context.Content.GetById(link.Udi);
                    if (content != null)
                    {
                        idProperty = new StringEnterspeedProperty(_entityIdentityService.GetId(content, culture));
                    }
                }
                else if (link.Udi.EntityType == "media")
                {
                    var media = context.Media.GetById(link.Udi);
                    if (media != null)
                    {
                        idProperty = new StringEnterspeedProperty(_entityIdentityService.GetId(media, culture));
                        url = _mediaUrlProvider.GetUrl(media);
                    }
                }
            }

            properties.Add("id", idProperty);
            properties.Add("url", new StringEnterspeedProperty(url));

            return new ObjectEnterspeedProperty(properties);
        }
    }
}
