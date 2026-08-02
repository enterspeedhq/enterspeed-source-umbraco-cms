using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.Base.Models.Api;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using NSubstitute;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Xunit;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Tests.Providers
{
    // Exercises the Umbraco 17+ flavour of the provider (the net10.0 branch), which is
    // where the async-only IDictionaryItemService/ILanguageService calls are bridged to
    // the package's sync call chains
    public class UmbracoLocalizationProviderTests
    {
        private readonly IDictionaryItemService _dictionaryItemService = Substitute.For<IDictionaryItemService>();
        private readonly ILanguageService _languageService = Substitute.For<ILanguageService>();

        private UmbracoLocalizationProvider CreateSut()
        {
            return new UmbracoLocalizationProvider(_dictionaryItemService, _languageService);
        }

        [Fact]
        public void GetDictionaryItem_ResolvesByKey()
        {
            var key = Guid.NewGuid();
            var dictionaryItem = Substitute.For<IDictionaryItem>();
            _dictionaryItemService.GetAsync(key).Returns(Task.FromResult(dictionaryItem));

            var result = CreateSut().GetDictionaryItem(key);

            Assert.Same(dictionaryItem, result);
        }

        [Fact]
        public void GetDictionaryItem_UnknownKey_ReturnsNull()
        {
            _dictionaryItemService.GetAsync(Arg.Any<Guid>()).Returns(Task.FromResult<IDictionaryItem>(null));

            var result = CreateSut().GetDictionaryItem(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public void GetDictionaryItem_SeedNodeWithKey_ResolvesByKey()
        {
            var key = Guid.NewGuid();
            var dictionaryItem = Substitute.For<IDictionaryItem>();
            _dictionaryItemService.GetAsync(key).Returns(Task.FromResult(dictionaryItem));

            var result = CreateSut().GetDictionaryItem(new CustomSeedNode { Id = 1234, Key = key });

            Assert.Same(dictionaryItem, result);
        }

        [Fact]
        public void GetDictionaryItem_SeedNodeWithoutKey_ReturnsNull()
        {
            var result = CreateSut().GetDictionaryItem(new CustomSeedNode { Id = 1234 });

            Assert.Null(result);
            _dictionaryItemService.DidNotReceive().GetAsync(Arg.Any<Guid>());
        }

        [Fact]
        public void GetDictionaryItemDescendants_PassesParentKeyThrough()
        {
            var parentKey = Guid.NewGuid();
            var descendants = new[] { Substitute.For<IDictionaryItem>(), Substitute.For<IDictionaryItem>() };
            _dictionaryItemService.GetDescendantsAsync(parentKey)
                .Returns(Task.FromResult<IEnumerable<IDictionaryItem>>(descendants));

            var result = CreateSut().GetDictionaryItemDescendants(parentKey);

            Assert.Equal(descendants, result);
        }

        [Fact]
        public void GetDictionaryItemDescendants_NullParent_ReturnsAllItems()
        {
            // The full-seed path asks for descendants of null, meaning every dictionary item
            var allItems = new[] { Substitute.For<IDictionaryItem>() };
            _dictionaryItemService.GetDescendantsAsync(null)
                .Returns(Task.FromResult<IEnumerable<IDictionaryItem>>(allItems));

            var result = CreateSut().GetDictionaryItemDescendants(null);

            Assert.Equal(allItems, result);
        }

        [Fact]
        public void GetAllLanguages_DelegatesToLanguageService()
        {
            var languages = new[] { Substitute.For<ILanguage>(), Substitute.For<ILanguage>() };
            _languageService.GetAllAsync().Returns(Task.FromResult<IEnumerable<ILanguage>>(languages));

            var result = CreateSut().GetAllLanguages();

            Assert.Equal(languages, result);
        }
    }
}
