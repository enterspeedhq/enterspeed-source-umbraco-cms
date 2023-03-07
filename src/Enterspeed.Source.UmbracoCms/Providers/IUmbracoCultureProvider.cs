using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Providers
{
    public interface IUmbracoCultureProvider
    {
        IEnumerable<string> GetCultures(IPublishedContent content);
        
        IEnumerable<string> GetCultures(IContent content);
    }
}