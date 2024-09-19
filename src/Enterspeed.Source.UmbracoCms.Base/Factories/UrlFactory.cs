using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.Options;
using StackExchange.Profiling.Internal;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Base.Factories
{
    public class UrlFactory : IUrlFactory
    {
        private readonly IUmbracoContextProvider _umbracoContextProvider;
        private readonly GlobalSettings _globalSettings;

        public UrlFactory(
            IUmbracoContextProvider umbracoContextProvider,
            IOptions<GlobalSettings> globalSettings)
        {
            _umbracoContextProvider = umbracoContextProvider;
            _globalSettings = globalSettings.Value;
        }

        public virtual string GetUrl(IPublishedContent content, bool preview, string culture)
        {
            var output = "#";
            var umb = _umbracoContextProvider.GetContext();
            var pathAsIds = content.Path
                .Split(',')
                .Select(int.Parse)
                .ToList();

            pathAsIds.Remove(content.Id);

            var ancestorsAndSelf = pathAsIds
                .Select(x => umb.Content.GetById(preview, x))
                .Where(x => x != null)
                .ToList();

            ancestorsAndSelf.Add(content);
            ancestorsAndSelf.Reverse();

            var domains = umb.Domains.GetAll(false).ToList();
            var urlSegments = new List<string>();
            
            foreach (var ancestor in ancestorsAndSelf)
            {
                var urlSegment = GetUrlSegment(culture, domains, ancestor, content.Path, out var isAssignedDomain);
                urlSegments.Add(urlSegment);
                if (isAssignedDomain)
                {
                    break;
                }
            }

            urlSegments.Reverse();

            if (urlSegments.Any(x => x == null))
            {
                return output;
            }

            var cleanedUrlArray = urlSegments.Select(x =>
            {
                var isSegmentAbsolute = Uri.TryCreate(x, UriKind.Absolute, out var segmentUri);
                if (isSegmentAbsolute)
                {
                    return x.Trim('/');
                }

                return x.Equals("/") ? x : x.Trim('/');
            });

            var url = string.Join("/", cleanedUrlArray);

            // Make sure theres only one slash at the start of a relative url
            if (Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                url = $"/{url.TrimStart('/')}";
            }

            return url.EnsureTrailingSlash();
        }

        protected virtual string GetUrlSegment(string culture, IEnumerable<Domain> domains, IPublishedContent ancestor, string path, out bool isAssignedDomain)
        {
            var assignedDomain = domains.FirstOrDefault(x => x.ContentId == ancestor.Id && x.Culture.Equals(culture, StringComparison.OrdinalIgnoreCase));
            if (assignedDomain != null)
            {
                if (assignedDomain.Name.StartsWith("/"))
                {
                    isAssignedDomain = true;
                    return assignedDomain.Name;
                }

                var isAbsolute = Uri.TryCreate(assignedDomain.Name, UriKind.Absolute, out var uri);
                if (isAbsolute)
                {
                    isAssignedDomain = true;
                    return assignedDomain.Name;
                }

                var output = _globalSettings.UseHttps
                    ? $"https://{assignedDomain.Name}"
                    : $"http://{assignedDomain.Name}";

                isAssignedDomain = true;
                return output;
            }

            var ancestorPath = ancestor.Path.Split(',');
            var originalPath = path.Split(',').Take(ancestorPath.Length);

            // Is site and should return / if nothing is configured in Cultures and Hostnames
            if (originalPath.Count() == 2)
            {
                isAssignedDomain = false;
                return "/";
            }

            try
            {
                var output = ancestor.UrlSegment(culture);
                isAssignedDomain = false;
                return output;
            }
            catch
            {
                isAssignedDomain = false;
                return null;
            }
        }
    }
}