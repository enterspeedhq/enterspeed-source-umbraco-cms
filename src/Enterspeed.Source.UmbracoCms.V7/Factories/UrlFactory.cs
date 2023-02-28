using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Strings;

namespace Enterspeed.Source.UmbracoCms.V7.Factories
{
    public class UrlFactory
    {
        public virtual string GetUrl(IContent content)
        {
            var output = string.Empty;

            var pathAsIds = content
                .Path
                .Split(',')
                .Select(int.Parse)
                .ToList();

            var ancestorPath = pathAsIds;
            ancestorPath.Remove(content.Id);

            var ancestorsAndSelf = ApplicationContext.Current.Services.ContentService.GetByIds(ancestorPath).ToList();

            ancestorsAndSelf.Add(content);
            ancestorsAndSelf.Reverse();

            var context = UmbracoContextHelper.GetUmbracoContext();
            var domains = context.Application.Services.DomainService.GetAll(false);
            var urlSegments = new List<string>();

            foreach (var ancestor in ancestorsAndSelf)
            {
                var isAssigned = false;
                var urlSegment = UrlSegmentProviderResolver.Current.Providers
                    .Select(provider =>
                    {
                        var segment = GetUrlSegment(provider, domains, ancestor, content.Path, out var isAssignedDomain);
                        isAssigned = isAssignedDomain;
                        return segment;
                    })
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                urlSegments.Add(urlSegment);

                if (isAssigned)
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
                var isSegmentAbsolute = Uri.TryCreate(x, UriKind.Absolute, out var segmeentUri);
                if (isSegmentAbsolute)
                {
                    return x.Trim('/');
                }

                return x.Trim('/');
            });

            var url = string.Join("/", cleanedUrlArray);
            if (string.IsNullOrWhiteSpace(url))
            {
                url = "/";
            }

            output = url.EnsureTrailingSlash();

            return output;
        }

        protected virtual string GetUrlSegment(IUrlSegmentProvider provider, IEnumerable<IDomain> domains, IContent ancestor, string path, out bool isAssignedDomain)
        {
            var assignedDomain = domains.FirstOrDefault(x => x.RootContentId == ancestor.Id);

            if (assignedDomain != null)
            {
                if (assignedDomain.DomainName.StartsWith("/"))
                {
                    isAssignedDomain = true;
                    return assignedDomain.DomainName;
                }

                var isAbsolute = Uri.TryCreate(assignedDomain.DomainName, UriKind.Absolute, out var uri);
                if (isAbsolute)
                {
                    isAssignedDomain = true;
                    return assignedDomain.DomainName;
                }

                var output = EnterspeedContext.Current.UmbracoGlobalSettings.UseHttps
                    ? $"https://{assignedDomain.DomainName}"
                    : $"http://{assignedDomain.DomainName}";

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
                isAssignedDomain = false;
                return provider.GetUrlSegment(ancestor);
            }
            catch
            {
                isAssignedDomain = false;
                return null;
            }
        }
    }
}
