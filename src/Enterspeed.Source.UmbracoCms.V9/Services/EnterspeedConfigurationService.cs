using System;
using System.Configuration;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Enterspeed.Source.UmbracoCms.V9.Models.Configuration;
using Enterspeed.Source.UmbracoCms.V9.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public class EnterspeedConfigurationService : IEnterspeedConfigurationService
    {
        private readonly IKeyValueService _keyValueService;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        private EnterspeedUmbracoConfiguration _enterspeedUmbracoConfiguration;

        [Obsolete("Use separate configuration keys instead.", false)]
        private readonly string _configurationDatabaseKey = "Enterspeed+Configuration";

        private readonly string _configurationMediaDomainDatabaseKey = "Enterspeed+Configuration+MediaDomain";
        private readonly string _configurationApiKeyDatabaseKey = "Enterspeed+Configuration+ApiKey";
        private readonly string _configurationPreviewApiKeyDatabaseKey = "Enterspeed+Configuration+PreviewApiKey";
        private readonly string _configurationConnectionTimeoutDatabaseKey =
            "Enterspeed+Configuration+ConnectionTimeout";
        private readonly string _configurationBaseUrlDatabaseKey = "Enterspeed+Configuration+BaseUrl";

        public EnterspeedConfigurationService(
            IKeyValueService keyValueService,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _keyValueService = keyValueService;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public EnterspeedUmbracoConfiguration GetConfiguration()
        {
            if (_enterspeedUmbracoConfiguration != null)
            {
                return _enterspeedUmbracoConfiguration;
            }

            _enterspeedUmbracoConfiguration = GetCombinedConfigurationFromDatabase();
            if (_enterspeedUmbracoConfiguration != null)
            {
                return _enterspeedUmbracoConfiguration;
            }

            _enterspeedUmbracoConfiguration = GetConfigurationFromDatabase();
            if (_enterspeedUmbracoConfiguration != null)
            {
                return _enterspeedUmbracoConfiguration;
            }

            _enterspeedUmbracoConfiguration = GetConfigurationFromSettingsFile();
            return _enterspeedUmbracoConfiguration;
        }

        public bool IsPublishConfigured()
        {
            var configuration = GetConfiguration();
            return configuration is { IsConfigured: true };
        }

        public bool IsPreviewConfigured()
        {
            var configuration = GetConfiguration();
            return configuration != null
                   && configuration.IsConfigured
                   && !string.IsNullOrWhiteSpace(configuration.PreviewApiKey);
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

            // Since old configuration single key is Obsolete and will be deprecated, transform it into newest version configuration, and cleanup obsolete version.
            configuration.IsConfigured = true;
            _keyValueService.SetValue(_configurationApiKeyDatabaseKey, configuration.ApiKey);
            _keyValueService.SetValue(_configurationBaseUrlDatabaseKey, configuration.BaseUrl);
            _keyValueService.SetValue(_configurationConnectionTimeoutDatabaseKey, configuration.ConnectionTimeout.ToString());
            _keyValueService.SetValue(_configurationMediaDomainDatabaseKey, configuration.MediaDomain);
            _keyValueService.SetValue(_configurationPreviewApiKeyDatabaseKey, configuration.PreviewApiKey);

            if (_keyValueService.GetValue(_configurationDatabaseKey) != null)
            {
                _keyValueService.SetValue(_configurationDatabaseKey, null);
            }

            _enterspeedUmbracoConfiguration = configuration;

            // Reinitialize connections in case of changes in the configuration
            var connectionProvider = _serviceProvider.GetRequiredService<IEnterspeedConnectionProvider>();
            connectionProvider.Initialize();
        }

        [Obsolete("Use GetCombinedConfigurationFromDatabase() instead.", false)]
        private EnterspeedUmbracoConfiguration GetConfigurationFromDatabase()
        {
            var savedConfigurationValue = _keyValueService.GetValue(_configurationDatabaseKey);

            if (string.IsNullOrWhiteSpace(savedConfigurationValue))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<EnterspeedUmbracoConfiguration>(savedConfigurationValue);
        }

        private EnterspeedUmbracoConfiguration GetConfigurationFromSettingsFile()
        {
            var webConfigEndpoint = _configuration["Enterspeed:Endpoint"];
            var webConfigMediaDomain = _configuration["Enterspeed:MediaDomain"];
            var webConfigApikey = _configuration["Enterspeed:Apikey"];
            var webConfigPreviewApikey = ConfigurationManager.AppSettings["Enterspeed.PreviewApikey"];

            if (string.IsNullOrWhiteSpace(webConfigEndpoint) || string.IsNullOrWhiteSpace(webConfigApikey))
            {
                return new EnterspeedUmbracoConfiguration();
            }

            _enterspeedUmbracoConfiguration = new EnterspeedUmbracoConfiguration
            {
                BaseUrl = webConfigEndpoint.Trim(),
                ApiKey = webConfigApikey.Trim(),
                MediaDomain = webConfigMediaDomain?.Trim(),
                IsConfigured = true,
                PreviewApiKey = webConfigPreviewApikey
            };

            return _enterspeedUmbracoConfiguration;
        }

        private EnterspeedUmbracoConfiguration GetCombinedConfigurationFromDatabase()
        {
            var apiKey = _keyValueService.GetValue(_configurationApiKeyDatabaseKey);
            var baseUrl = _keyValueService.GetValue(_configurationBaseUrlDatabaseKey);

            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            var mediaDomain = _keyValueService.GetValue(_configurationMediaDomainDatabaseKey);
            var connectionTimeoutAsString = _keyValueService.GetValue(_configurationConnectionTimeoutDatabaseKey);
            var previewApiKey = _keyValueService.GetValue(_configurationPreviewApiKeyDatabaseKey);

            var configuration = new EnterspeedUmbracoConfiguration
            {
                IsConfigured = true,
                ApiKey = apiKey,
                BaseUrl = baseUrl,
                MediaDomain = mediaDomain,
                PreviewApiKey = previewApiKey
            };

            if (int.TryParse(connectionTimeoutAsString, out var connectionTimeout))
            {
                configuration.ConnectionTimeout = connectionTimeout;
            }

            return configuration;
        }
    }
}