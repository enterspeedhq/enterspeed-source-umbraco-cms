using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.Base.Models.Api;
using Enterspeed.Source.UmbracoCms.V14Plus.Models;
using NSubstitute;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Xunit;
using CustomSeedNode = Enterspeed.Source.UmbracoCms.V14Plus.Models.CustomSeedNode;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Tests.Services
{
    public class EnterspeedJobServiceCustomSeedTests
    {
        private readonly Base.Services.IEnterspeedJobService _baseJobService = Substitute.For<Base.Services.IEnterspeedJobService>();
        private readonly IContentService _contentService = Substitute.For<IContentService>();
        private readonly IMediaService _mediaService = Substitute.For<IMediaService>();
        private readonly IDictionaryItemService _dictionaryItemService = Substitute.For<IDictionaryItemService>();

        private V14Plus.Services.EnterspeedJobService CreateSut()
        {
            return new V14Plus.Services.EnterspeedJobService(
                _baseJobService, _contentService, _mediaService, _dictionaryItemService);
        }

        private static CustomSeedModel CreateSeedModel(params CustomSeedNode[] dictionaryNodes)
        {
            return new CustomSeedModel
            {
                ContentNodes = new List<CustomSeedNode>(),
                MediaNodes = new List<CustomSeedNode>(),
                DictionaryNodes = new List<CustomSeedNode>(dictionaryNodes),
            };
        }

        [Fact]
        public async Task CustomSeed_DictionaryNode_CarriesGuidKeyAlongsideIntId()
        {
            // Umbraco 18 removed the int-keyed dictionary lookup, so the Guid key
            // must travel with the seed node (implementation plan section 4.2)
            var key = Guid.NewGuid();
            var dictionaryItem = Substitute.For<IDictionaryItem>();
            dictionaryItem.Id.Returns(42);
            dictionaryItem.Key.Returns(key);
            _dictionaryItemService.GetAsync(key).Returns(Task.FromResult(dictionaryItem));

            CustomSeed captured = null;
            _baseJobService.CustomSeed(true, false, Arg.Do<CustomSeed>(x => captured = x));

            await CreateSut().CustomSeed(
                CreateSeedModel(new CustomSeedNode { Id = key.ToString(), IncludeDescendants = true }),
                true,
                false);

            Assert.NotNull(captured);
            var node = Assert.Single(captured.DictionaryNodes);
            Assert.Equal(42, node.Id);
            Assert.Equal(key, node.Key);
            Assert.True(node.IncludeDescendants);
        }

        [Fact]
        public async Task CustomSeed_UnknownDictionaryNode_IsSkipped()
        {
            _dictionaryItemService.GetAsync(Arg.Any<Guid>()).Returns(Task.FromResult<IDictionaryItem>(null));

            CustomSeed captured = null;
            _baseJobService.CustomSeed(true, false, Arg.Do<CustomSeed>(x => captured = x));

            await CreateSut().CustomSeed(
                CreateSeedModel(new CustomSeedNode { Id = Guid.NewGuid().ToString() }),
                true,
                false);

            Assert.NotNull(captured);
            Assert.Empty(captured.DictionaryNodes);
        }

        [Fact]
        public async Task CustomSeed_EverythingDictionaryNode_MapsToMinusOneWithoutKey()
        {
            CustomSeed captured = null;
            _baseJobService.CustomSeed(true, false, Arg.Do<CustomSeed>(x => captured = x));

            await CreateSut().CustomSeed(
                CreateSeedModel(new CustomSeedNode { Id = "Everything" }),
                true,
                false);

            Assert.NotNull(captured);
            var node = Assert.Single(captured.DictionaryNodes);
            Assert.Equal(-1, node.Id);
            Assert.Null(node.Key);
            Assert.True(node.IncludeDescendants);
        }
    }
}
