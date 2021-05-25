using System;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.Sdk.Domain.SystemTextJson;
using Enterspeed.Source.UmbracoCms.V7.Services;

namespace Enterspeed.Source.UmbracoCms.V7.Contexts
{
    public class EnterspeedServiceContext
    {
        private readonly Lazy<EnterspeedConfigurationService> _configurationService;

        public EnterspeedServiceContext()
        {
            _configurationService = new Lazy<EnterspeedConfigurationService>(() => new EnterspeedConfigurationService());
        }

        public EnterspeedConfigurationService ConfigurationService => _configurationService.Value;
        public EnterspeedJobService JobService => new EnterspeedJobService();
        public EntityIdentityService EntityIdentityService => new EntityIdentityService();
        public EnterspeedPropertyService PropertyService => new EnterspeedPropertyService();
        public EnterspeedGridEditorService GridEditorService => new EnterspeedGridEditorService();

        public IEnterspeedIngestService IngestService => new EnterspeedIngestService(
            EnterspeedContext.Current.Connection,
            new SystemTextJsonSerializer(),
            EnterspeedContext.Current.Providers.ConfigurationProvider);
    }
}
