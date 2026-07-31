#if UMBRACO_18_OR_GREATER
using System;
using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters;
using NSubstitute;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Xunit;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Tests.Services.DataProperties
{
    public class DefaultSingleBlockPropertyValueConverterTests
    {
        private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();
        private readonly IEnterspeedPropertyService _propertyService = Substitute.For<IEnterspeedPropertyService>();
        private readonly DefaultSingleBlockPropertyValueConverter _sut;

        public DefaultSingleBlockPropertyValueConverterTests()
        {
            _serviceProvider.GetService(typeof(IEnterspeedPropertyService)).Returns(_propertyService);
            _sut = new DefaultSingleBlockPropertyValueConverter(_serviceProvider);
        }

        [Theory]
        [InlineData("Umbraco.SingleBlock", true)]
        [InlineData("Umbraco.BlockList", false)]
        [InlineData("Umbraco.BlockGrid", false)]
        public void IsConverter_MatchesOnlyTheSingleBlockAlias(string editorAlias, bool expected)
        {
            var propertyType = Substitute.For<IPublishedPropertyType>();
            propertyType.EditorAlias.Returns(editorAlias);

            Assert.Equal(expected, _sut.IsConverter(propertyType));
        }

        [Fact]
        public void Convert_BlockListItem_MapsContentAndContentTypeLikeSingleModeBlockList()
        {
            // The v18 upgrade rewrites single-mode Block Lists to Umbraco.SingleBlock;
            // the payload shape must match what the block list converter produced
            var contentType = Substitute.For<IPublishedContentType>();
            contentType.Alias.Returns("heroBlock");

            var content = Substitute.For<IPublishedElement>();
            content.ContentType.Returns(contentType);
            content.Properties.Returns(new List<IPublishedProperty>());

            var headlineProperty = new StringEnterspeedProperty("headline", "Hello");
            _propertyService.ConvertProperties(content.Properties, null)
                .Returns(new Dictionary<string, IEnterspeedProperty> { ["headline"] = headlineProperty });

            var property = CreateProperty("hero");
            property.GetValue(null, null).Returns(new BlockListItem(Guid.NewGuid(), content, null, null));

            var result = Assert.IsType<ObjectEnterspeedProperty>(_sut.Convert(property, null));

            Assert.Equal("heroBlock", ((StringEnterspeedProperty)result.Properties["contentType"]).Value);
            var mappedContent = Assert.IsType<ObjectEnterspeedProperty>(result.Properties["content"]);
            Assert.Same(headlineProperty, mappedContent.Properties["headline"]);
            Assert.False(result.Properties.ContainsKey("settings"));
        }

        [Fact]
        public void Convert_NoValue_ReturnsEmptyArray()
        {
            var property = CreateProperty("hero");
            property.GetValue(null, null).Returns(null);

            var result = Assert.IsType<ArrayEnterspeedProperty>(_sut.Convert(property, null));

            Assert.Equal("hero", result.Name);
            Assert.Empty(result.Items);
        }

        private static IPublishedProperty CreateProperty(string alias)
        {
            var propertyType = Substitute.For<IPublishedPropertyType>();
            propertyType.EditorAlias.Returns("Umbraco.SingleBlock");

            var property = Substitute.For<IPublishedProperty>();
            property.Alias.Returns(alias);
            property.PropertyType.Returns(propertyType);
            return property;
        }
    }
}
#endif
