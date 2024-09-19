using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    public interface IUmbracoCultureProvider
    {
        IEnumerable<string> GetCulturesForCultureVariant(IContent content);
        IEnumerable<string> GetCulturesForCultureVariant(IPublishedContent content);
        string GetCultureForNonCultureVariant(IContent content);
        string GetCultureForNonCultureVariant(IPublishedContent content);
    }
}