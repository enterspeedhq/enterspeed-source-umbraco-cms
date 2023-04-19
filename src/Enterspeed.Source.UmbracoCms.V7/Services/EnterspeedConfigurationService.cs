using System.Configuration;
using Enterspeed.Source.UmbracoCms.V7.Data.Schemas;
using Enterspeed.Source.UmbracoCms.V7.Extensions;
using Enterspeed.Source.UmbracoCms.V7.Models.Configuration;
using Umbraco.Core;
using Umbraco.Core.Configuration;

namespace Enterspeed.Source.UmbracoCms.V7.Services
{
    public class EnterspeedConfigurationService
    {
        private EnterspeedUmbracoConfiguration _configuration;

        private static string GetUmbracoVersion()
        {
            return $"Umbraco-{UmbracoVersion.Current.Major}.{UmbracoVersion.Current.Minor}.{UmbracoVersion.Current.Build}";
        }

        public EnterspeedUmbracoConfiguration GetConfiguration()
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
            var webConfigPreviewApikey = ConfigurationManager.AppSettings["Enterspeed.PreviewApikey"];

            if (string.IsNullOrWhiteSpace(webConfigEndpoint) || string.IsNullOrWhiteSpace(webConfigApikey))
            {
                return new EnterspeedUmbracoConfiguration();
            }

            _configuration = new EnterspeedUmbracoConfiguration
            {
                BaseUrl = webConfigEndpoint.Trim(),
                ApiKey = webConfigApikey.Trim(),
                MediaDomain = webConfigMediaDomain?.Trim(),
                IsConfigured = true,
                PreviewApiKey = webConfigPreviewApikey?.Trim(),
                SystemInformation = GetUmbracoVersion()
            };
            return _configuration;
        }

        public void Save(EnterspeedUmbracoConfiguration configuration)
        {
            if (configuration == null)
            {
                return;
            }

            configuration.IsConfigured = true;
            configuration.MediaDomain = configuration.MediaDomain.TrimEnd('/');
            if (!configuration.MediaDomain.IsAbsoluteUrl())
            {
                throw new ConfigurationErrorsException(
                    "Configuration value for Enterspeed.MediaDomain must be absolute url");
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                // In case of SQL CE we need to see if the config is already existing, so we can update it, or otherwise insert it
                var existingConfig = GetConfigurationFromDatabase();
                if (existingConfig == null)
                {
                    db.Insert(MapToSchema(configuration));
                }
                else
                {
                    db.Save(MapToSchema(configuration));
                }

                db.CompleteTransaction();
                db.CloseSharedConnection();
            }

            _configuration = configuration;
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

        private EnterspeedUmbracoConfiguration GetConfigurationFromDatabase()
        {
            EnterspeedConfigurationSchema configuration;
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                configuration = db.FirstOrDefault<EnterspeedConfigurationSchema>("SELECT TOP 1 * FROM [EnterspeedConfiguration]");
                db.CompleteTransaction();
                db.CloseSharedConnection();
            }

            if (configuration == null)
            {
                return null;
            }

            return MapFromSchema(configuration);
        }

        private EnterspeedConfigurationSchema MapToSchema(EnterspeedUmbracoConfiguration config)
        {
            if (config == null)
            {
                return null;
            }

            return new EnterspeedConfigurationSchema
            {
                Id = 1, // Default 1
                ApiKey = config.ApiKey,
                ConnectionTimeout = config.ConnectionTimeout,
                MediaDomain = config.MediaDomain,
                BaseUrl = config.BaseUrl,
                IngestVersion = config.IngestVersion,
                PreviewApiKey = config.PreviewApiKey
            };
        }

        private EnterspeedUmbracoConfiguration MapFromSchema(EnterspeedConfigurationSchema config)
        {
            if (config == null)
            {
                return null;
            }

            return new EnterspeedUmbracoConfiguration
            {
                ApiKey = config.ApiKey,
                ConnectionTimeout = config.ConnectionTimeout,
                MediaDomain = config.MediaDomain,
                BaseUrl = config.BaseUrl,
                IsConfigured = true,
                PreviewApiKey = config.PreviewApiKey,
                SystemInformation = GetUmbracoVersion()
            };
        }
    }
}