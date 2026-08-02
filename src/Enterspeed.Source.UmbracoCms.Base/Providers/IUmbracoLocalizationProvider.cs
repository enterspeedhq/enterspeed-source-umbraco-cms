using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    /// <summary>
    /// Wraps the dictionary and language lookups behind one interface so consumers do not
    /// need to know which Umbraco service generation they run against: ILocalizationService
    /// on Umbraco 9-16, ILanguageService/IDictionaryItemService on Umbraco 17+ (where
    /// ILocalizationService is removed).
    /// </summary>
    public interface IUmbracoLocalizationProvider
    {
        IDictionaryItem GetDictionaryItem(Guid key);

#if !NET10_0_OR_GREATER
        // Umbraco 17+ has no int-keyed dictionary lookup; int-keyed callers only exist on
        // the older package lines, where the dashboard sends integer ids
        IDictionaryItem GetDictionaryItem(int id);
#endif

        IEnumerable<IDictionaryItem> GetDictionaryItemDescendants(Guid? parentKey);

        IEnumerable<ILanguage> GetAllLanguages();
    }
}
