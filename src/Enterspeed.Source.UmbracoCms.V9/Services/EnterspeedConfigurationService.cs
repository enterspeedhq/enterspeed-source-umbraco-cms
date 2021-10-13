using System.Configuration;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Enterspeed.Source.UmbracoCms.V9.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public class EnterspeedConfigurationService : IEnterspeedConfigurationService
    {
        private readonly IKeyValueService _keyValueService;
        private readonly IConfiguration _configuration;
        
        private EnterspeedUmbracoConfiguration _enterspeedUmbracoConfiguration;
        private readonly string _configurationDatabaseKey = "Enterspeed+Configuration";

        public EnterspeedConfigurationService(
            IKeyValueService keyValueService,
            IConfiguration configuration)
        {
            _keyValueService = keyValueService;
            _configuration = configuration;
        }

        public EnterspeedUmbracoConfiguration GetConfiguration()
        {
            if (_enterspeedUmbracoConfiguration != null)
            {
                return _enterspeedUmbracoConfiguration;
            }

            _enterspeedUmbracoConfiguration = GetConfigurationFromDatabase();

            if (_enterspeedUmbracoConfiguration != null)
            {
                return _enterspeedUmbracoConfiguration;
            }

            var webConfigEndpoint = _configuration["Enterspeed:Endpoint"];
            var webConfigMediaDomain = _configuration["Enterspeed:MediaDomain"];
            var webConfigApikey = _configuration["Enterspeed:Apikey"];

            if (string.IsNullOrWhiteSpace(webConfigEndpoint) || string.IsNullOrWhiteSpace(webConfigApikey))
            {
                return new EnterspeedUmbracoConfiguration();
            }

            _enterspeedUmbracoConfiguration = new EnterspeedUmbracoConfiguration
            {
                BaseUrl = webConfigEndpoint?.Trim(),
                ApiKey = webConfigApikey?.Trim(),
                MediaDomain = webConfigMediaDomain?.Trim(),
                IsConfigured = true
            };

            return _enterspeedUmbracoConfiguration;
        }

        public void Save(EnterspeedUmbracoConfiguration configuration)
        {
            if (configuration == null)
            {
                return;
            }

            configuration.MediaDomain = configuration.MediaDomain.TrimEnd('/');
            if (!configuration.MediaDomain.IsAbsoluteUrl())
            {
                throw new ConfigurationErrorsException(
                    "Configuration value for Enterspeed.MediaDomain must be absolute url");
            }

            configuration.IsConfigured = true;

            _keyValueService.SetValue(_configurationDatabaseKey, JsonConvert.SerializeObject(configuration));
            _enterspeedUmbracoConfiguration = configuration;
        }

        private EnterspeedUmbracoConfiguration GetConfigurationFromDatabase()
        {
            var savedConfigurationValue = _keyValueService.GetValue(_configurationDatabaseKey);

            if (string.IsNullOrWhiteSpace(savedConfigurationValue))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<EnterspeedUmbracoConfiguration>(savedConfigurationValue);
        }
    }
}
