using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public class UmbracoUrlService : IUmbracoUrlService
    {
        private readonly IUmbracoContextProvider _contextProvider;

        public UmbracoUrlService(
            IUmbracoContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public string GetUrlFromIdUrl(string idUrl, string culture)
        {
            var output = idUrl;

            var id = GetIdFromIdUrl(idUrl);

            if (id <= 0)
            {
                return output;
            }

            var umbContext = _contextProvider.GetContext();
            var domainsFromId = umbContext.Domains.GetAssigned(id, false);

            var domain = domainsFromId.FirstOrDefault(x => x.Culture.ToLower().Equals(culture.ToLower()));
            if (domain == null)
            {
                return idUrl.Replace(id.ToString(), string.Empty);
            }

            var domainUrl = idUrl.Replace(id.ToString(), domain.Name);

            output = PrepareUrl(domainUrl);

            return output;
        }

        public int GetIdFromIdUrl(string idUrl)
        {
            if (string.IsNullOrWhiteSpace(idUrl))
            {
                return 0;
            }

            const string idFromUrlPattern = @"^(\d+)/?(.*)";
            var regexMatch = Regex.Match(idUrl, idFromUrlPattern);
            if (regexMatch.Success && regexMatch.Groups.Count >= 1)
            {
                var rootContentId = int.Parse(regexMatch.Groups[1].Value);

                return rootContentId;
            }

            return 0;
        }

        private string PrepareUrl(string url)
        {
            if (url.EndsWith("/"))
            {
                url = url.Remove(url.Length - 1);
            }

            var includesProtocol = Uri.TryCreate(url, UriKind.Absolute, out var uriResult);

            // TODO: global settings + request settingds
            // if (!includesProtocol && !url.StartsWith("/"))
            // {
            //     var protocol = _globalSettings.UseHttps ? "https" : "http";
            //     url = $"{protocol}://{url}";
            // }
            //
            // if (_requestSettings.AddTrailingSlash)
            // {
            //     url += "/";
            // }

            return url;
        }
    }
}
