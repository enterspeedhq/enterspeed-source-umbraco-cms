using System;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.UmbracoCms.V7.Services;
using Enterspeed.Source.UmbracoCms.V7.Services.Serializers;

namespace Enterspeed.Source.UmbracoCms.V7.Contexts
{
    public class EnterspeedServiceContext
    {
        private readonly Lazy<EnterspeedConfigurationService> _configurationService;
        private SchedulerService _schedulerService;

        public EnterspeedServiceContext()
        {
            _configurationService = new Lazy<EnterspeedConfigurationService>(() => new EnterspeedConfigurationService());
        }

        public EnterspeedConfigurationService ConfigurationService => _configurationService.Value;
        public EnterspeedJobService JobService => new EnterspeedJobService();
        public EntityIdentityService EntityIdentityService => new EntityIdentityService();
        public EnterspeedPropertyService PropertyService => new EnterspeedPropertyService();
        public EnterspeedGridEditorService GridEditorService => new EnterspeedGridEditorService();
        public SchedulerService SchedulerService => _schedulerService ?? (_schedulerService = new SchedulerService());

        public IEnterspeedIngestService IngestService => new EnterspeedIngestService(
            EnterspeedContext.Current.Connection,
            new NewtonSoftJsonSerializer(),
            EnterspeedContext.Current.Providers.ConfigurationProvider);
    }
}
