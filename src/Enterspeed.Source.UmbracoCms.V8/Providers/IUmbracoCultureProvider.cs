using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Providers
{
    public interface IUmbracoCultureProvider
    {
        IEnumerable<string> GetCulturesForCultureVariant(IContent content);
        IEnumerable<string> GetCulturesForCultureVariant(IPublishedContent content);
        string GetCultureForNonCultureVariant(IContent content);
        string GetCultureForNonCultureVariant(IPublishedContent content);
    }
}