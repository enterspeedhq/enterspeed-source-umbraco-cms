using Enterspeed.Source.UmbracoCms.Models.Configuration;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public interface IEnterspeedConfigurationService
    {
        void Save(EnterspeedUmbracoConfiguration configuration);
        EnterspeedUmbracoConfiguration GetConfiguration();
        bool IsPublishConfigured();
        bool IsPreviewConfigured();
    }
}