using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class EnterspeedPropertyService : IEnterspeedPropertyService
    {
        private const string MetaData = "metaData";
        private readonly EnterspeedPropertyValueConverterCollection _converterCollection;
        private readonly IEntityIdentityService _identityService;

        public EnterspeedPropertyService(EnterspeedPropertyValueConverterCollection converterCollection)
        {
            _converterCollection = converterCollection;
            _identityService = Current.Factory.GetInstance<IEntityIdentityService>();
        }

        public IDictionary<string, IEnterspeedProperty> GetProperties(IPublishedContent content, string culture = null)
        {
            var properties = content.Properties;
            var enterspeedProperties = ConvertProperties(properties, culture);

            enterspeedProperties.Add(MetaData, CreateMetaData(content, culture));

            return enterspeedProperties;
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
            var enterspeedProperties = new Dictionary<string, IEnterspeedProperty>
            {
                { "name", new StringEnterspeedProperty("name", media.Name) },
                { "size", new StringEnterspeedProperty("size", media.GetValue<string>("umbracoBytes")) },
                { "type", new StringEnterspeedProperty("type", media.GetValue<string>("umbracoExtension")) },
                { "path", new StringEnterspeedProperty("path", media.Path) },
                { "createDate", new StringEnterspeedProperty("createDate", media.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss")) },
                { "updateDate", new StringEnterspeedProperty("updateDate", media.UpdateDate.ToString("yyyy-MM-ddTHH:mm:ss")) },
                { "level", new NumberEnterspeedProperty("level", media.Level) },
                { "nodePath", new ArrayEnterspeedProperty("nodePath", GetNodePath(media)) },
            };

            return enterspeedProperties;
        }

        private IEnterspeedProperty CreateMetaData(IPublishedContent content, string culture)
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

            return new ObjectEnterspeedProperty("metaData", metaData);
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
                .Select(x => new StringEnterspeedProperty(null, _identityService.GetId(media)))
                .ToArray();
        }
    }
}
