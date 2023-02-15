using System;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.UmbracoCms.Extensions;
using Enterspeed.Source.UmbracoCms.Models.Configuration;
using Enterspeed.Source.UmbracoCms.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public class EnterspeedConfigurationService : IEnterspeedConfigurationService
    {
        private readonly IKeyValueService _keyValueService;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        private EnterspeedUmbracoConfiguration _enterspeedUmbracoConfiguration;

        private const string ConfigurationMediaDomainDatabaseKey = "Enterspeed+Configuration+MediaDomain";
        private const string ConfigurationApiKeyDatabaseKey = "Enterspeed+Configuration+ApiKey";
        private const string ConfigurationPreviewApiKeyDatabaseKey = "Enterspeed+Configuration+PreviewApiKey";
        private const string ConfigurationConnectionTimeoutDatabaseKey = "Enterspeed+Configuration+ConnectionTimeout";
        private const string ConfigurationBaseUrlDatabaseKey = "Enterspeed+Configuration+BaseUrl";

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

            _enterspeedUmbracoConfiguration = GetConfigurationFromSettingsFile();
            if (_enterspeedUmbracoConfiguration != null)
            {
                return _enterspeedUmbracoConfiguration;
            }

            _enterspeedUmbracoConfiguration = GetConfigurationFromDatabase();

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
            return configuration is { IsConfigured: true } 
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
                throw new ConfigurationException("Configuration value for Enterspeed.MediaDomain must be absolute url");
            }
            
            configuration.IsConfigured = true;
            _keyValueService.SetValue(ConfigurationApiKeyDatabaseKey, configuration.ApiKey);
            _keyValueService.SetValue(ConfigurationBaseUrlDatabaseKey, configuration.BaseUrl);
            _keyValueService.SetValue(ConfigurationConnectionTimeoutDatabaseKey, configuration.ConnectionTimeout.ToString());
            _keyValueService.SetValue(ConfigurationMediaDomainDatabaseKey, configuration.MediaDomain);
            _keyValueService.SetValue(ConfigurationPreviewApiKeyDatabaseKey, configuration.PreviewApiKey);

            _enterspeedUmbracoConfiguration = configuration;

            // Reinitialize connections in case of changes in the configuration
            var connectionProvider = _serviceProvider.GetRequiredService<IEnterspeedConnectionProvider>();
            connectionProvider.Initialize();
        }

        private EnterspeedUmbracoConfiguration GetConfigurationFromSettingsFile()
        {
            var endpoint = _configuration["Enterspeed:Endpoint"]; // The naming for endpoint does not match the baseUrl property so this property is mapped manually
            var configuration = _configuration.GetSection("Enterspeed").Get<EnterspeedUmbracoConfiguration>();

            if (configuration == null || string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(configuration.ApiKey))
            {
                return null;
            }

            configuration.BaseUrl = endpoint;
            configuration.IsConfigured = true;
            configuration.ConfiguredFromSettingsFile = true;

            return configuration;
        }

        private EnterspeedUmbracoConfiguration GetConfigurationFromDatabase()
        {
            var apiKey = _keyValueService.GetValue(ConfigurationApiKeyDatabaseKey);
            var baseUrl = _keyValueService.GetValue(ConfigurationBaseUrlDatabaseKey);

            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            var mediaDomain = _keyValueService.GetValue(ConfigurationMediaDomainDatabaseKey);
            var connectionTimeoutAsString = _keyValueService.GetValue(ConfigurationConnectionTimeoutDatabaseKey);
            var previewApiKey = _keyValueService.GetValue(ConfigurationPreviewApiKeyDatabaseKey);

            var configuration = new EnterspeedUmbracoConfiguration
            {
                IsConfigured = true,
                ConfiguredFromSettingsFile = false,
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