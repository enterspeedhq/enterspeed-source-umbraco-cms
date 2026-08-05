using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Models.Api;
using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    /// <summary>
    /// Wraps the dictionary and language lookups behind one interface so consumers do not
    /// need to know which Umbraco service generation they run against: ILocalizationService
    /// on Umbraco 9-16, ILanguageService/IDictionaryItemService on Umbraco 17+. Umbraco 18
    /// removed ILocalizationService; Umbraco 17 only marks it obsolete, but shares the
    /// net10.0 target, so both use the newer services.
    /// </summary>
    public interface IUmbracoLocalizationProvider
    {
        IDictionaryItem GetDictionaryItem(Guid key);

        IDictionaryItem GetDictionaryItem(CustomSeedNode seedNode);

        IEnumerable<IDictionaryItem> GetDictionaryItemDescendants(Guid? parentKey);

        IEnumerable<ILanguage> GetAllLanguages();
    }
}
