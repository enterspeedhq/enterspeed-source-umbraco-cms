#if UMBRACO_18_OR_GREATER
using System;
using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters;
using NSubstitute;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Xunit;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Tests.Services.DataProperties
{
    public class DefaultElementPickerPropertyValueConverterTests
    {
        private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();
        private readonly IEnterspeedPropertyService _propertyService = Substitute.For<IEnterspeedPropertyService>();
        private readonly DefaultElementPickerPropertyValueConverter _sut;

        public DefaultElementPickerPropertyValueConverterTests()
        {
            _serviceProvider.GetService(typeof(IEnterspeedPropertyService)).Returns(_propertyService);
            _sut = new DefaultElementPickerPropertyValueConverter(_serviceProvider);
        }

        [Theory]
        [InlineData("Umbraco.ElementPicker", true)]
        [InlineData("Umbraco.ContentPicker", false)]
        [InlineData("Umbraco.BlockList", false)]
        public void IsConverter_MatchesOnlyTheElementPickerAlias(string editorAlias, bool expected)
        {
            var propertyType = Substitute.For<IPublishedPropertyType>();
            propertyType.EditorAlias.Returns(editorAlias);

            Assert.Equal(expected, _sut.IsConverter(propertyType));
        }

        [Fact]
        public void Convert_MapsResolvedElementsToKeyContentTypeAndContent()
        {
            var property = CreateProperty("relatedElements");

            var key = Guid.NewGuid();
            var element = CreateElement(key, "textElement");
            var headingProperty = new StringEnterspeedProperty("heading", "Hello");
            _propertyService.ConvertProperties(element.Properties, null)
                .Returns(new Dictionary<string, IEnterspeedProperty> { ["heading"] = headingProperty });

            property.GetValue(null, null).Returns(new List<IPublishedElement> { element });

            var result = Assert.IsType<ArrayEnterspeedProperty>(_sut.Convert(property, null));

            Assert.Equal("relatedElements", result.Name);
            var item = Assert.IsType<ObjectEnterspeedProperty>(Assert.Single(result.Items));
            Assert.Equal(key.ToString(), ((StringEnterspeedProperty)item.Properties["key"]).Value);
            Assert.Equal("textElement", ((StringEnterspeedProperty)item.Properties["contentType"]).Value);
            var content = Assert.IsType<ObjectEnterspeedProperty>(item.Properties["content"]);
            Assert.Same(headingProperty, content.Properties["heading"]);
        }

        [Fact]
        public void Convert_MultipleElements_PreservesOrder()
        {
            var property = CreateProperty("relatedElements");

            var first = CreateElement(Guid.NewGuid(), "textElement");
            var second = CreateElement(Guid.NewGuid(), "imageElement");
            _propertyService.ConvertProperties(Arg.Any<IEnumerable<IPublishedProperty>>(), null)
                .Returns(new Dictionary<string, IEnterspeedProperty>());

            property.GetValue(null, null).Returns(new List<IPublishedElement> { first, second });

            var result = Assert.IsType<ArrayEnterspeedProperty>(_sut.Convert(property, null));

            Assert.Equal(2, result.Items.Length);
            Assert.Equal("textElement", ((StringEnterspeedProperty)((ObjectEnterspeedProperty)result.Items[0]).Properties["contentType"]).Value);
            Assert.Equal("imageElement", ((StringEnterspeedProperty)((ObjectEnterspeedProperty)result.Items[1]).Properties["contentType"]).Value);
        }

        [Fact]
        public void Convert_NoValue_ReturnsEmptyArray()
        {
            // The core converter already omits deleted/unpublished elements; a page
            // whose picked elements are all gone resolves to null/empty, never throws
            var property = CreateProperty("relatedElements");
            property.GetValue(null, null).Returns(null);

            var result = Assert.IsType<ArrayEnterspeedProperty>(_sut.Convert(property, null));

            Assert.Empty(result.Items);
        }

        private static IPublishedProperty CreateProperty(string alias)
        {
            var propertyType = Substitute.For<IPublishedPropertyType>();
            propertyType.EditorAlias.Returns("Umbraco.ElementPicker");
            propertyType.Variations.Returns(ContentVariation.Nothing);

            var property = Substitute.For<IPublishedProperty>();
            property.Alias.Returns(alias);
            property.PropertyType.Returns(propertyType);
            return property;
        }

        private static IPublishedElement CreateElement(Guid key, string contentTypeAlias)
        {
            var contentType = Substitute.For<IPublishedContentType>();
            contentType.Alias.Returns(contentTypeAlias);

            var element = Substitute.For<IPublishedElement>();
            element.Key.Returns(key);
            element.ContentType.Returns(contentType);
            element.Properties.Returns(new List<IPublishedProperty>());
            return element;
        }
    }
}
#endif
