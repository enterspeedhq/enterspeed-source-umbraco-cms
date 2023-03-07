using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Providers
{
    public class UmbracoCultureProvider : IUmbracoCultureProvider
    {
        private readonly IUmbracoContextProvider _umbracoContextProvider;

        public UmbracoCultureProvider(
            IUmbracoContextProvider umbracoContextProvider)
        {
            _umbracoContextProvider = umbracoContextProvider;
        }
            
        public IEnumerable<string> GetCultures(IPublishedContent content)
        {
            return content.ContentType.VariesByCulture()
                ? content.Cultures.Keys
                : GetCulturesFromDomain(content.Path);
        }

        public IEnumerable<string> GetCultures(IContent content)
        {
            return content.ContentType.VariesByCulture()
                ? content.AvailableCultures
                : GetCulturesFromDomain(content.Path);
        }

        private IEnumerable<string> GetCulturesFromDomain(string nodePath)
        {
            var umbracoContext = _umbracoContextProvider.GetContext();
            var domains = umbracoContext.Domains.GetAll(false).ToList();
            if (domains.Any())
            {
                var pathAsIds = nodePath
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                pathAsIds.Reverse();

                foreach (var ancestorId in pathAsIds)
                {
                    var assignedDomain = domains.Where(x => x.ContentId == ancestorId)
                        .Select(c => c.Culture)
                        .ToList();
                    
                    if (assignedDomain.Any())
                    {
                        return assignedDomain.AsEnumerable();
                    }
                }
            }

            return new List<string> { GetDefaultCulture(umbracoContext) };
        }

        private string GetDefaultCulture(IUmbracoContext umbracoContext)
        {
            return umbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }
    }
}