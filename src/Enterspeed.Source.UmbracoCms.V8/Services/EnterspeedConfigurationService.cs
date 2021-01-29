using System.Configuration;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Newtonsoft.Json;
using Umbraco.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class EnterspeedConfigurationService : IEnterspeedConfigurationService
    {
        private readonly IKeyValueService _keyValueService;
        private EnterspeedConfiguration _configuration;
        private readonly string _configurationDatabaseKey = "Enterspeed+Configuration";

        public EnterspeedConfigurationService(IKeyValueService keyValueService)
        {
            _keyValueService = keyValueService;
        }

        public EnterspeedConfiguration GetConfiguration()
        {
            if (_configuration != null)
            {
                return _configuration;
            }

            _configuration = GetConfigurationFromDatabase();
            if (_configuration != null)
            {
                return _configuration;
            }

            var webConfigEndpoint = ConfigurationManager.AppSettings["Enterspeed.Endpoint"];
            var webConfigMediaDomain = ConfigurationManager.AppSettings["Enterspeed.MediaDomain"];
            var webConfigApikey = ConfigurationManager.AppSettings["Enterspeed.Apikey"];
            _configuration = new EnterspeedConfiguration
            {
                BaseUrl = webConfigEndpoint?.Trim(),
                ApiKey = webConfigApikey?.Trim(),
                MediaDomain = webConfigMediaDomain?.Trim()
            };
            return _configuration;
        }

        public void Save(EnterspeedConfiguration configuration)
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

            _keyValueService.SetValue(_configurationDatabaseKey, JsonConvert.SerializeObject(configuration));
            _configuration = configuration;
        }

        private EnterspeedConfiguration GetConfigurationFromDatabase()
        {
            var savedConfigurationValue = _keyValueService.GetValue(_configurationDatabaseKey);
            if (string.IsNullOrWhiteSpace(savedConfigurationValue))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<EnterspeedConfiguration>(savedConfigurationValue);
        }
    }
}
