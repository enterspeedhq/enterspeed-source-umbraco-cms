using Enterspeed.Source.UmbracoCms.V8.Models.Configuration;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEnterspeedConfigurationService
    {
        void Save(EnterspeedUmbracoConfiguration configuration);
        EnterspeedUmbracoConfiguration GetConfiguration();
        bool IsPublishConfigured();
        bool IsPreviewConfigured();
    }
}