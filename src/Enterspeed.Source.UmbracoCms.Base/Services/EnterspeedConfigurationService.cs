using System;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Enterspeed.Source.UmbracoCms.Base.Models.Configuration;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public class EnterspeedConfigurationService : IEnterspeedConfigurationService
    {
        private readonly IKeyValueService _keyValueService;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUmbracoVersion _umbracoVersion;

        private EnterspeedUmbracoConfiguration _enterspeedUmbracoConfiguration;

        private const string ConfigurationMediaDomainDatabaseKey = "Enterspeed+Configuration+MediaDomain";
        private const string ConfigurationApiKeyDatabaseKey = "Enterspeed+Configuration+ApiKey";
        private const string ConfigurationPreviewApiKeyDatabaseKey = "Enterspeed+Configuration+PreviewApiKey";
        private const string ConfigurationConnectionTimeoutDatabaseKey = "Enterspeed+Configuration+ConnectionTimeout";
        private const string ConfigurationBaseUrlDatabaseKey = "Enterspeed+Configuration+BaseUrl";

        public EnterspeedConfigurationService(
            IKeyValueService keyValueService,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            IUmbracoVersion umbracoVersion)
        {
            _keyValueService = keyValueService;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _umbracoVersion = umbracoVersion;
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

            return _enterspeedUmbracoConfiguration ?? new EnterspeedUmbracoConfiguration();
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

        public bool IsRootDictionariesDisabled()
        {
            var configuration = GetConfiguration();
            return configuration.RootDictionariesDisabled;
        }

        public bool IsMasterContentDisabled()
        {
            var configuration = GetConfiguration();
            return configuration.EnableMasterContent;
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
            var configuration = _configuration.GetSection("Enterspeed").Get<EnterspeedUmbracoConfiguration>();

            if (configuration == null || string.IsNullOrWhiteSpace(configuration.ApiKey))
            {
                return null;
            }

            configuration.BaseUrl = GetBaseUrlFromSettingsFile(configuration);
            configuration.IsConfigured = true;
            configuration.ConfiguredFromSettingsFile = true;
            configuration.SystemInformation = GetUmbracoVersion();

            return configuration;
        }

        private string GetBaseUrlFromSettingsFile(EnterspeedUmbracoConfiguration configuration)
        {
            // Start checking for a custom BaseUrl
            var baseUrl = _configuration["Enterspeed:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                return baseUrl;
            }

            // Secondly check for a custom Endpoint for backwards compatibility
            var endpoint = _configuration["Enterspeed:Endpoint"];
            if (!string.IsNullOrWhiteSpace(endpoint))
            {
                return endpoint;
            }

            // If no custom values are provided then you the default one
            return configuration.BaseUrl;
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
                SystemInformation = GetUmbracoVersion(),
                ApiKey = apiKey,
                BaseUrl = baseUrl,
                MediaDomain = mediaDomain,
                PreviewApiKey = previewApiKey
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
            var enterspeedSection = _configuration.GetSection("Enterspeed");
            configuration.RootDictionariesDisabled = enterspeedSection.GetValue<bool>("RootDictionariesDisabled");
            configuration.RunJobsOnAllServerRoles = enterspeedSection.GetValue<bool>("RunJobsOnAllServerRoles");
            configuration.EnableMasterContent = enterspeedSection.GetValue<bool>("EnableMasterContent");
            configuration.EnabledFailedJobsProcessing = enterspeedSection.GetValue<bool>("EnabledFailedJobsProcessing");
            configuration.RemoveTrailingSlash = enterspeedSection.GetValue<bool>("RemoveTrailingSlash");
        }

        private string GetUmbracoVersion()
        {
            return $"Umbraco-{_umbracoVersion.Version}";
        }
    }
}