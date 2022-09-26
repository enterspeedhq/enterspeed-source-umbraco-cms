using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class EnterspeedPropertyService : IEnterspeedPropertyService
    {
        private const string MetaData = "metaData";
        private readonly EnterspeedPropertyValueConverterCollection _converterCollection;
        private readonly IEntityIdentityService _identityService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public EnterspeedPropertyService(
            EnterspeedPropertyValueConverterCollection converterCollection,
            IUmbracoContextFactory umbracoContextFactory)
        {
            _converterCollection = converterCollection;
            _umbracoContextFactory = umbracoContextFactory;
            _identityService = Current.Factory.GetInstance<IEntityIdentityService>();
        }

        public IDictionary<string, IEnterspeedProperty> GetProperties(IPublishedContent content, string culture = null)
        {
            var properties = content.Properties;
            var enterspeedProperties = ConvertProperties(properties, culture);

            enterspeedProperties.Add(MetaData, CreateNodeMetaData(content, culture));

            return enterspeedProperties;
        }

        private IEnterspeedProperty CreateNodeMetaData(IPublishedContent content, string culture)
        {
            var metaData = new Dictionary<string, IEnterspeedProperty>
            {
                ["name"] = new StringEnterspeedProperty("name", content.Name(culture)),
                ["culture"] = new StringEnterspeedProperty("culture", culture),
                ["sortOrder"] = new NumberEnterspeedProperty("sortOrder", content.SortOrder),
                ["level"] = new NumberEnterspeedProperty("level", content.Level),
                ["createDate"] = new StringEnterspeedProperty("createDate", content.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                ["updateDate"] = new StringEnterspeedProperty("updateDate", content.CultureDate(culture).ToString("yyyy-MM-ddTHH:mm:ss")),
                ["nodePath"] = new ArrayEnterspeedProperty("nodePath", GetNodePath(content.Path, culture))
            };

            MapAdditionalMetaData(metaData, content, culture);

            return new ObjectEnterspeedProperty("metaData", metaData);
        }

        public IDictionary<string, IEnterspeedProperty> ConvertProperties(IEnumerable<IPublishedProperty> properties, string culture = null)
        {
            var output = new Dictionary<string, IEnterspeedProperty>();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var converter = _converterCollection.FirstOrDefault(x => x.IsConverter(property.PropertyType));

                    var value = converter?.Convert(property, culture);

                    if (value == null)
                    {
                        continue;
                    }

                    output.Add(property.Alias, value);
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

                output.Add("key", new StringEnterspeedProperty(dictionaryItem.ItemKey));
                output.Add("translation", new StringEnterspeedProperty(value ?? string.Empty));
                output.Add("culture", new StringEnterspeedProperty(culture));
            }

            return output;
        }

        public IDictionary<string, IEnterspeedProperty> GetProperties(IMedia media)
        {
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var publishedMedia = context.UmbracoContext.Media?.GetById(media.Id);
                if (publishedMedia == null)
                {
                    return null;
                }

                var properties = publishedMedia.Properties.Where(p => !p.Alias.Equals(Constants.Conventions.Media.File));
                var enterspeedProperties = ConvertProperties(properties);
                enterspeedProperties.Add(MetaData, CreateMediaMetaProperties(media, publishedMedia));

                return enterspeedProperties;
            }
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

            MapAdditionalMetaData(metaData, publishedMedia, string.Empty);

            var metaProperties = new ObjectEnterspeedProperty(MetaData, metaData);
            return metaProperties;
        }

        /// <summary>
        /// Override to add extra meta data
        /// </summary>
        /// <param name="metaData"></param>
        protected virtual void MapAdditionalMetaData(Dictionary<string, IEnterspeedProperty> metaData, IPublishedContent content, string culture)
        {
        }

        private IEnterspeedProperty[] GetNodePath(string contentPath, string culture)
        {
            var ids = contentPath
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            ids.Remove(-1);

            return ids
                .Select(x => new StringEnterspeedProperty(null, _identityService.GetId(x, culture)))
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
