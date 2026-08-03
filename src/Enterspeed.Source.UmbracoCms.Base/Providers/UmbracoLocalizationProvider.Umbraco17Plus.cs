#if NET10_0_OR_GREATER
using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Models.Api;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    /// <summary>
    /// Umbraco 17+ flavour: dictionary items and languages come from IDictionaryItemService
    /// and ILanguageService (Umbraco 18 removed ILocalizationService; 17 marks it obsolete).
    /// The new services are async-only; the sync bridging lives here and only here.
    /// </summary>
    public class UmbracoLocalizationProvider : IUmbracoLocalizationProvider
    {
        private readonly IDictionaryItemService _dictionaryItemService;
        private readonly ILanguageService _languageService;

        public UmbracoLocalizationProvider(
            IDictionaryItemService dictionaryItemService,
            ILanguageService languageService)
        {
            _dictionaryItemService = dictionaryItemService;
            _languageService = languageService;
        }

        public IDictionaryItem GetDictionaryItem(Guid key)
        {
            return _dictionaryItemService.GetAsync(key).GetAwaiter().GetResult();
        }

        public IDictionaryItem GetDictionaryItem(CustomSeedNode seedNode)
        {
            return seedNode.Key.HasValue ? GetDictionaryItem(seedNode.Key.Value) : null;
        }

        public IEnumerable<IDictionaryItem> GetDictionaryItemDescendants(Guid? parentKey)
        {
            return _dictionaryItemService.GetDescendantsAsync(parentKey).GetAwaiter().GetResult();
        }

        public IEnumerable<ILanguage> GetAllLanguages()
        {
            return _languageService.GetAllAsync().GetAwaiter().GetResult();
        }
    }
}
#endif
