using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Models;
using Enterspeed.Source.UmbracoCms.V7.Services.DataProperties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services
{
    public class EnterspeedPropertyService
    {
        private const string MetaData = "metaData";
        private readonly OrderedCollection<IEnterspeedPropertyValueConverter> _converterCollection;
        private readonly EntityIdentityService _identityService;

        public EnterspeedPropertyService()
        {
            _converterCollection = EnterspeedContext.Current.EnterspeedPropertyValueConverters;
            _identityService = EnterspeedContext.Current.Services.EntityIdentityService;
        }

        public IDictionary<string, IEnterspeedProperty> GetProperties(IPublishedContent content)
        {
            var enterspeedProperties = ConvertProperties(content);

            enterspeedProperties.Add(MetaData, CreateMetaData(content));

            return enterspeedProperties;
        }

        public IDictionary<string, IEnterspeedProperty> ConvertProperties(IPublishedContent content, IEnumerable<IPublishedProperty> filteredProperties = null)
        {
            var output = new Dictionary<string, IEnterspeedProperty>();
            var properties = filteredProperties ?? content.Properties;
            var propertyTypes = content.ContentType.PropertyTypes?.ToList();

            if (properties != null && propertyTypes != null)
            {
                foreach (var property in properties)
                {
                    var propertyType =
                        propertyTypes.FirstOrDefault(x => x.PropertyTypeAlias == property.PropertyTypeAlias);
                    if (propertyType == null)
                    {
                        continue;
                    }

                    var converter = _converterCollection.FirstOrDefault(x => x.IsConverter(propertyType));

                    var value = converter?.Convert(property);

                    if (value == null)
                    {
                        continue;
                    }

                    output.Add(property.PropertyTypeAlias, value);
                }
            }

            return output;
        }

        public IDictionary<string, IEnterspeedProperty> GetProperties(IDictionaryItem dictionaryItem, string culture)
        {
            var output = new Dictionary<string, IEnterspeedProperty>();

            if (dictionaryItem?.Translations != null)
            {
                var value = dictionaryItem.Translations
                    .FirstOrDefault(x => x.Language.IsoCode.Equals(culture, StringComparison.OrdinalIgnoreCase))?.Value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    output.Add("key", new StringEnterspeedProperty(dictionaryItem.ItemKey));
                    output.Add("translation", new StringEnterspeedProperty(value));
                    output.Add("culture", new StringEnterspeedProperty(culture));
                }
            }

            return output;
        }

        public IDictionary<string, IEnterspeedProperty> GetProperties(IMedia media)
        {
            var context = UmbracoContextHelper.GetUmbracoContext();

            var publishedMedia = context.MediaCache.GetById(media.Id);
            if (publishedMedia == null)
            {
                return null;
            }

            var filteredProperties = publishedMedia.Properties.Where(p => !p.PropertyTypeAlias.Equals(Constants.Conventions.Media.File));
            var enterspeedProperties = ConvertProperties(publishedMedia, filteredProperties);
            enterspeedProperties.Add(MetaData, CreateMediaMetaProperties(media, publishedMedia));

            return enterspeedProperties;
        }

        private ObjectEnterspeedProperty CreateMediaMetaProperties(IMedia media, IPublishedContent publishedMedia)
        {
            var metaData = new Dictionary<string, IEnterspeedProperty>
            {
                { "name", new StringEnterspeedProperty("name", media.Name) },
                { "size", new StringEnterspeedProperty("size", media.GetValue<string>("umbracoBytes")) },
                { "type", new StringEnterspeedProperty("type", media.GetValue<string>("umbracoExtension")) },
                { "width", new StringEnterspeedProperty("width", media.GetValue<int>("umbracoWidth").ToString()) },
                { "height", new StringEnterspeedProperty("height", media.GetValue<int>("umbracoHeight").ToString()) },
                { "path", new StringEnterspeedProperty("path", media.Path) },
                { "createDate", new StringEnterspeedProperty("createDate", media.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss")) },
                { "updateDate", new StringEnterspeedProperty("updateDate", media.UpdateDate.ToString("yyyy-MM-ddTHH:mm:ss")) },
                { "level", new NumberEnterspeedProperty("level", media.Level) },
                { "nodePath", new ArrayEnterspeedProperty("nodePath", GetNodePath(media)) },
            };

            var metaProperties = new ObjectEnterspeedProperty(MetaData, metaData);
            return metaProperties;
        }

        private IEnterspeedProperty CreateMetaData(IPublishedContent content)
        {
            var metaData = new Dictionary<string, IEnterspeedProperty>
            {
                ["name"] = new StringEnterspeedProperty("name", content.Name),
                ["culture"] = new StringEnterspeedProperty("culture", content.GetCulture().IetfLanguageTag.ToLowerInvariant()),
                ["sortOrder"] = new NumberEnterspeedProperty("sortOrder", content.SortOrder),
                ["level"] = new NumberEnterspeedProperty("level", content.Level),
                ["createDate"] = new StringEnterspeedProperty("createDate", content.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                ["updateDate"] = new StringEnterspeedProperty("updateDate", content.UpdateDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                ["nodePath"] = new ArrayEnterspeedProperty("nodePath", GetNodePath(content.Path))
            };

            return new ObjectEnterspeedProperty("metaData", metaData);
        }

        private IEnterspeedProperty[] GetNodePath(string contentPath)
        {
            var ids = contentPath
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            ids.Remove(-1);

            return ids
                .Select(x => new StringEnterspeedProperty(null, _identityService.GetId(x)))
                .ToArray();
        }

        private IEnterspeedProperty[] GetNodePath(IMedia media)
        {
            var ids = media.Path
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            ids.Remove(-1);

            return ids
                .Select(x => new StringEnterspeedProperty(null, _identityService.GetId(x)))
                .ToArray();
        }
    }
}
