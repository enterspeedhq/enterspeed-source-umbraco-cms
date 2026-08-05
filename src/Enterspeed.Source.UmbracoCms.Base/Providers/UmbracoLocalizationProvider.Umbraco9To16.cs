#if !NET10_0_OR_GREATER
using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Models.Api;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    /// <summary>
    /// Umbraco 9-16 flavour: dictionary items and languages come from ILocalizationService.
    /// </summary>
    public class UmbracoLocalizationProvider : IUmbracoLocalizationProvider
    {
        private readonly ILocalizationService _localizationService;

        public UmbracoLocalizationProvider(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public IDictionaryItem GetDictionaryItem(Guid key)
        {
            return _localizationService.GetDictionaryItemById(key);
        }

        public IDictionaryItem GetDictionaryItem(CustomSeedNode seedNode)
        {
            return seedNode.Key.HasValue
                ? _localizationService.GetDictionaryItemById(seedNode.Key.Value)
                : _localizationService.GetDictionaryItemById(seedNode.Id);
        }

        public IEnumerable<IDictionaryItem> GetDictionaryItemDescendants(Guid? parentKey)
        {
            return _localizationService.GetDictionaryItemDescendants(parentKey);
        }

        public IEnumerable<ILanguage> GetAllLanguages()
        {
            return _localizationService.GetAllLanguages();
        }
    }
}
#endif
