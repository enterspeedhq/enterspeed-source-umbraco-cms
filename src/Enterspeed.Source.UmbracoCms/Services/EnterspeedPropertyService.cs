using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.DataPropertyValueConverters;
using Enterspeed.Source.UmbracoCms.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public class EnterspeedPropertyService : IEnterspeedPropertyService
    {
        private const string MetaData = "metaData";
        private readonly EnterspeedPropertyValueConverterCollection _converterCollection;
        private readonly IEntityIdentityService _identityService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly ILogger<EnterspeedPropertyService> _logger;
        private readonly IDomainService _domainService;
        private readonly IEnterspeedValidationService _enterspeedValidationService;

        public EnterspeedPropertyService(
            EnterspeedPropertyValueConverterCollection converterCollection,
            IServiceProvider serviceProvider,
            IUmbracoContextFactory umbracoContextFactory,
            ILogger<EnterspeedPropertyService> logger,
            IDomainService domainService,
            IEnterspeedValidationService enterspeedValidationService)
        {
            _converterCollection = converterCollection;
            _umbracoContextFactory = umbracoContextFactory;
            _logger = logger;
            _domainService = domainService;
            _identityService = serviceProvider.GetRequiredService<IEntityIdentityService>();
            _enterspeedValidationService = enterspeedValidationService;
        }

        public IDictionary<string, IEnterspeedProperty> GetProperties(IPublishedContent content, string culture = null)
        {
            var properties = content.Properties;
            var enterspeedProperties = ConvertProperties(properties, culture);

            MapAdditionalProperties((Dictionary<string, IEnterspeedProperty>)enterspeedProperties, content, culture);

            enterspeedProperties.Add(MetaData, CreateNodeMetaData(content, culture));

            _enterspeedValidationService.LogValidationErrors(enterspeedProperties);

            return enterspeedProperties;
        }

        private IEnterspeedProperty CreateNodeMetaData(IPublishedContent content, string culture)
        {
            var metaData = new Dictionary<string, IEnterspeedProperty>
            {
                ["name"] = new StringEnterspeedProperty("name", content.Name(culture)),
                ["culture"] = new StringEnterspeedProperty("culture", culture),
                ["domain"] = new StringEnterspeedProperty("domain", GetDomain(content, culture)?.DomainName),
                ["sortOrder"] = new NumberEnterspeedProperty("sortOrder", content.SortOrder),
                ["level"] = new NumberEnterspeedProperty("level", content.Level),
                ["createDate"] = new StringEnterspeedProperty("createDate", content.CreateDate.ToEnterspeedFormatString()),
                ["updateDate"] = new StringEnterspeedProperty("updateDate", content.CultureDate(culture).ToEnterspeedFormatString()),
                ["nodePath"] = new ArrayEnterspeedProperty("nodePath", GetNodePath(content.Path, culture))
            };

            MapAdditionalMetaData(metaData, content, culture);

            return new ObjectEnterspeedProperty("metaData", metaData);
        }

        private IDomain GetDomain(IPublishedContent content, string culture)
        {
            var domain = _domainService.GetAssignedDomains(content.Id, false)
                ?.FirstOrDefault(p => string.Equals(p.LanguageIsoCode, culture, StringComparison.InvariantCultureIgnoreCase));
            return domain;
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
            using var context = _umbracoContextFactory.EnsureUmbracoContext();

            IDictionary<string, IEnterspeedProperty> enterspeedProperties;

            var publishedMedia = context.UmbracoContext.Media?.GetById(media.Id);
            if (publishedMedia != null)
            {
                var properties = publishedMedia.Properties.Where(p => !p.Alias.Equals(Constants.Conventions.Media.File));
                enterspeedProperties = ConvertProperties(properties);
            }
            else
            {
                _logger.LogWarning($"Could not get media as published content, for media with id of {media.Id}");
                enterspeedProperties = new Dictionary<string, IEnterspeedProperty>();
            }

            MapAdditionalMediaProperties((Dictionary<string, IEnterspeedProperty>)enterspeedProperties, publishedMedia, string.Empty);

            enterspeedProperties.Add(MetaData, CreateMediaMetaProperties(media, publishedMedia));

            _enterspeedValidationService.LogValidationErrors(enterspeedProperties);

            return enterspeedProperties;
        }

        private ObjectEnterspeedProperty CreateMediaMetaProperties(IMedia media, IPublishedContent publishedMedia)
        {
            var metaData = new Dictionary<string, IEnterspeedProperty>
            {
                { "name", new StringEnterspeedProperty("name", media.Name) },
                { "path", new StringEnterspeedProperty("path", media.Path) },
                { "createDate", new StringEnterspeedProperty("createDate", media.CreateDate.ToEnterspeedFormatString()) },
                { "updateDate", new StringEnterspeedProperty("updateDate", media.UpdateDate.ToEnterspeedFormatString()) },
                { "level", new NumberEnterspeedProperty("level", media.Level) },
                { "nodePath", new ArrayEnterspeedProperty("nodePath", GetNodePath(media)) },
            };

            if (media.ContentType.Name == "Image")
            {
                metaData.Add("size", new StringEnterspeedProperty("size", media.GetValue<string>("umbracoBytes")));
                metaData.Add("width", new StringEnterspeedProperty("width", media.GetValue<int>("umbracoWidth").ToString()));
                metaData.Add("height", new StringEnterspeedProperty("height", media.GetValue<int>("umbracoHeight").ToString()));
                metaData.Add("contentType", new StringEnterspeedProperty("contentType", media.GetValue<string>("umbracoExtension")));
            }

            MapAdditionalMediaMetaData(metaData, publishedMedia, string.Empty);

            var metaProperties = new ObjectEnterspeedProperty(MetaData, metaData);
            return metaProperties;
        }

        /// <summary>
        /// Override to add extra properties on content
        /// </summary>
        /// <param name="data"></param>
        /// <param name="content"></param>
        /// <param name="culture"></param>
        protected virtual void MapAdditionalProperties(Dictionary<string, IEnterspeedProperty> data, IPublishedContent content, string culture)
        {
        }

        /// <summary>
        /// Override to add extra properties on media
        /// </summary>
        /// <param name="data"></param>
        /// <param name="content"></param>
        /// <param name="culture"></param>
        protected virtual void MapAdditionalMediaProperties(Dictionary<string, IEnterspeedProperty> data, IPublishedContent content, string culture)
        {
        }

        /// <summary>
        /// Override to add extra meta data on content
        /// </summary>
        /// <param name="metaData"></param>
        /// <param name="content"></param>
        /// <param name="culture"></param>
        protected virtual void MapAdditionalMetaData(Dictionary<string, IEnterspeedProperty> metaData, IPublishedContent content, string culture)
        {
        }

        /// <summary>
        /// Override to add extra meta data on media
        /// </summary>
        /// <param name="metaData"></param>
        /// <param name="content"></param>
        /// <param name="culture"></param>
        protected virtual void MapAdditionalMediaMetaData(Dictionary<string, IEnterspeedProperty> metaData, IPublishedContent content, string culture)
        {
        }

        private IEnterspeedProperty[] GetNodePath(string contentPath, string culture)
        {
            var ids = contentPath
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            ids.Remove(-1);

            return ids.Select(x => new StringEnterspeedProperty(null, _identityService.GetId(x, culture))).ToArray();
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