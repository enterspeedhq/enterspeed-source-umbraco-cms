using Enterspeed.Source.UmbracoCms.V10.Models.Configuration;

namespace Enterspeed.Source.UmbracoCms.V10.Services
{
    public interface IEnterspeedConfigurationService
    {
        void Save(EnterspeedUmbracoConfiguration configuration);
        EnterspeedUmbracoConfiguration GetConfiguration();
        bool IsPublishConfigured();
        bool IsPreviewConfigured();
    }
}