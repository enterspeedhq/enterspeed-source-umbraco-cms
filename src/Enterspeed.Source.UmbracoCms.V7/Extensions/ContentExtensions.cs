using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Models;
using Umbraco.Web.Routing;
using PublishedProperty = Enterspeed.Source.UmbracoCms.V7.Models.UmbracoModels.PublishedProperty;

namespace Enterspeed.Source.UmbracoCms.V7.Extensions
{
    public static class ContentExtensions
    {
        /// <summary>
        /// Convert an IContent to an IPublishedContent.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="isPreview">
        /// The is preview.
        /// </param>
        /// <returns>
        /// The <see cref="IPublishedContent"/>.
        /// </returns>
        public static IPublishedContent ToPublishedContent(this IContent content, bool isPreview = false)
        {
            return content != null ? new PublishedContent(content, isPreview) : null;
        }
    }

    /// <summary>
    /// The published content.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class PublishedContent : PublishedContentWithKeyBase
    {
        private readonly PublishedContentType _contentType;

        private readonly IContent _inner;
        private readonly bool _isPreviewing;
        private readonly Lazy<string> _lazyCreatorName;
        private readonly Lazy<string> _lazyUrlName;
        private readonly Lazy<string> _lazyWriterName;
        private readonly Lazy<string> _lazyUrl;

        private readonly IPublishedProperty[] _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedContent"/> class.
        /// </summary>
        /// <param name="inner">
        /// The inner.
        /// </param>
        /// <param name="isPreviewing">
        /// The is previewing.
        /// </param>
        public PublishedContent(IContent inner, bool isPreviewing)
        {
            if (inner == null)
            {
                return;
            }

            _inner = inner;
            _isPreviewing = isPreviewing;

            _lazyUrlName = new Lazy<string>(() => _inner.GetUrlSegment().ToLower());
            _lazyCreatorName = new Lazy<string>(() => _inner.GetCreatorProfile(ApplicationContext.Current.Services.UserService)?.Name);
            _lazyWriterName = new Lazy<string>(() => _inner.GetWriterProfile(ApplicationContext.Current.Services.UserService)?.Name);
            _lazyUrl = new Lazy<string>(() =>
            {
                return EnterspeedContext.Current.Services.UrlFactory.GetUrl(inner);
            });

            _contentType = PublishedContentType.Get(PublishedItemType.Content, _inner.ContentType.Alias);

            _properties =
                MapProperties(
                    _contentType.PropertyTypes,
                    _inner.Properties,
                    (t, v) => new PublishedProperty(t, v, _isPreviewing)).ToArray();
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public override int Id => _inner.Id;

        public override string Url => _lazyUrl.Value;

        /// <summary>
        /// Gets the key.
        /// </summary>
        public override Guid Key => _inner.Key;

        /// <summary>
        /// Gets the document type id.
        /// </summary>
        public override int DocumentTypeId => _inner.ContentTypeId;

        /// <summary>
        /// Gets the document type alias.
        /// </summary>
        public override string DocumentTypeAlias => _inner.ContentType.Alias;

        /// <summary>
        /// Gets the item type.
        /// </summary>
        public override PublishedItemType ItemType => PublishedItemType.Content;

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name => _inner.Name;

        /// <summary>
        /// Gets the level.
        /// </summary>
        public override int Level => _inner.Level;

        /// <summary>
        /// Gets the path.
        /// </summary>
        public override string Path => _inner.Path;

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        public override int SortOrder => _inner.SortOrder;

        /// <summary>
        /// Gets the version.
        /// </summary>
        public override Guid Version => _inner.Version;

        /// <summary>
        /// Gets the template id.
        /// </summary>
        public override int TemplateId => _inner.Template?.Id ?? 0;

        /// <summary>
        /// Gets the url name.
        /// </summary>
        public override string UrlName => _lazyUrlName.Value;

        /// <summary>
        /// Gets the create date.
        /// </summary>
        public override DateTime CreateDate => _inner.CreateDate;

        /// <summary>
        /// Gets the update date.
        /// </summary>
        public override DateTime UpdateDate => _inner.UpdateDate;

        /// <summary>
        /// Gets the creator id.
        /// </summary>
        public override int CreatorId => _inner.CreatorId;

        /// <summary>
        /// Gets the creator name.
        /// </summary>
        public override string CreatorName => _lazyCreatorName.Value;

        /// <summary>
        /// Gets the writer id.
        /// </summary>
        public override int WriterId => _inner.WriterId;

        /// <summary>
        /// Gets the writer name.
        /// </summary>
        public override string WriterName => _lazyWriterName.Value;

        /// <summary>
        /// Gets a value indicating whether is draft.
        /// </summary>
        public override bool IsDraft => _inner.Published == false;

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public override IPublishedContent Parent
        {
            get
            {
                var parent = _inner.Parent();
                return parent.ToPublishedContent(_isPreviewing);
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public override IEnumerable<IPublishedContent> Children
        {
            get
            {
                var children = _inner.Children().ToList();

                return
                    children.Select(x => x.ToPublishedContent(_isPreviewing))
                        .Where(x => x != null)
                        .OrderBy(x => x.SortOrder);
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public override ICollection<IPublishedProperty> Properties => _properties;

        /// <summary>
        /// Gets the content type.
        /// </summary>
        public override PublishedContentType ContentType => _contentType;

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="IPublishedProperty"/>.
        /// </returns>
        public override IPublishedProperty GetProperty(string alias)
        {
            return _properties.FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));
        }

        /// <summary>
        /// The map properties.
        /// </summary>
        /// <param name="propertyTypes">
        /// The property types.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        internal static IEnumerable<IPublishedProperty> MapProperties(
            IEnumerable<PublishedPropertyType> propertyTypes,
            IEnumerable<Property> properties,
            Func<PublishedPropertyType, object, IPublishedProperty> map)
        {
            var propertyEditorResolver = PropertyEditorResolver.Current;
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

            return propertyTypes.Select(
                x =>
                {
                    var p = properties.SingleOrDefault(xx => xx.Alias == x.PropertyTypeAlias);
                    var v = p == null || p.Value == null ? null : p.Value;
                    if (v != null)
                    {
                        var e = propertyEditorResolver.GetByAlias(x.PropertyEditorAlias);

                        // We are converting to string, even for database values which are integer or
                        // DateTime, which is not optimum. Doing differently would require that we have a way to tell
                        // whether the conversion to XML string changes something or not... which we don't, and we
                        // don't want to implement it as PropertyValueEditor.ConvertDbToXml/String should die anyway.

                        // Don't think about improving the situation here: this is a corner case and the real
                        // thing to do is to get rig of PropertyValueEditor.ConvertDbToXml/String.

                        // Use ConvertDbToString to keep it simple, although everywhere we use ConvertDbToXml and
                        // nothing ensures that the two methods are consistent.
                        if (e != null)
                        {
                            v = e.ValueEditor.ConvertDbToString(p, p.PropertyType, dataTypeService);
                        }
                    }

                    return map(x, v);
                });
        }
    }
}
