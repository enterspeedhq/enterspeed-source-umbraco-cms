using Enterspeed.Source.UmbracoCms.NetCore.Models.Configuration;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services
{
    public interface IEnterspeedConfigurationService
    {
        void Save(EnterspeedUmbracoConfiguration configuration);
        EnterspeedUmbracoConfiguration GetConfiguration();
        bool IsPublishConfigured();
        bool IsPreviewConfigured();
    }
}