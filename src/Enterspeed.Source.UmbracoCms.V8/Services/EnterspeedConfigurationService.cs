using System;
using System.Configuration;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Enterspeed.Source.UmbracoCms.V8.Models.Configuration;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Services;
using Umbraco.Core.Sync;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class EnterspeedConfigurationService : IEnterspeedConfigurationService
    {
        private readonly IKeyValueService _keyValueService;
        private EnterspeedUmbracoConfiguration _configuration;

        [Obsolete("Use separate configuration keys instead.", false)]
        private readonly string _configurationDatabaseKey = "Enterspeed+Configuration";

        private readonly string _configurationMediaDomainDatabaseKey = "Enterspeed+Configuration+MediaDomain";
        private readonly string _configurationApiKeyDatabaseKey = "Enterspeed+Configuration+ApiKey";
        private readonly string _configurationPreviewApiKeyDatabaseKey = "Enterspeed+Configuration+PreviewApiKey";
        private readonly string _configurationConnectionTimeoutDatabaseKey = "Enterspeed+Configuration+ConnectionTimeout";
        private readonly string _configurationBaseUrlDatabaseKey = "Enterspeed+Configuration+BaseUrl";

        public EnterspeedConfigurationService(IKeyValueService keyValueService)
        {
            _keyValueService = keyValueService;
        }

        public EnterspeedUmbracoConfiguration GetConfiguration()
        {
            if (_configuration != null)
            {
                return _configuration;
            }

            _configuration = GetCombinedConfigurationFromDatabase();
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
            var webConfigPreviewApikey = ConfigurationManager.AppSettings["Enterspeed.PreviewApikey"];

            if (string.IsNullOrWhiteSpace(webConfigEndpoint) || string.IsNullOrWhiteSpace(webConfigApikey))
            {
                return new EnterspeedUmbracoConfiguration();
            }

            _configuration = new EnterspeedUmbracoConfiguration
            {
                BaseUrl = webConfigEndpoint?.Trim(),
                ApiKey = webConfigApikey?.Trim(),
                MediaDomain = webConfigMediaDomain?.Trim(),
                IsConfigured = true,
                PreviewApiKey = webConfigPreviewApikey?.Trim(),
                SystemInformation = GetUmbracoVersion()
            };

            SetOptionalSettings(_configuration);

            return _configuration;
        }

        public bool IsPublishConfigured()
        {
            var configuration = GetConfiguration();
            return configuration != null && configuration.IsConfigured;
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

            _configuration = configuration;

            // Reinitialize connections in case of changes in the configuration
            var connectionProvider = Current.Factory.GetInstance<IEnterspeedConnectionProvider>();
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

            var configuration = JsonConvert.DeserializeObject<EnterspeedUmbracoConfiguration>(savedConfigurationValue);

            if (configuration != null)
            {
                configuration.SystemInformation = GetUmbracoVersion();
                SetOptionalSettings(configuration);
            }

            return configuration;
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
                PreviewApiKey = previewApiKey,
                SystemInformation = GetUmbracoVersion()
            };

            if (int.TryParse(connectionTimeoutAsString, out var connectionTimeout))
            {
                configuration.ConnectionTimeout = connectionTimeout;
            }

            SetOptionalSettings(configuration);

            return configuration;
        }

        private void SetOptionalSettings(EnterspeedUmbracoConfiguration configuration)
        {
            bool.TryParse(ConfigurationManager.AppSettings["Enterspeed.RunJobsOnAllServerRoles"], out var runJobsOnAllServerRoles);
            configuration.RunJobsOnAllServerRoles = runJobsOnAllServerRoles;
        }

        private static string GetUmbracoVersion()
        {
            return $"Umbraco-{UmbracoVersion.Current.Major}.{UmbracoVersion.Current.Minor}.{UmbracoVersion.Current.Build}";
        }
    }
}
