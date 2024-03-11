using Enterspeed.Source.UmbracoCms.Core.Models.Configuration;

namespace Enterspeed.Source.UmbracoCms.Core.Services
{
    public interface IEnterspeedConfigurationService
    {
        void Save(EnterspeedUmbracoConfiguration configuration);
        EnterspeedUmbracoConfiguration GetConfiguration();
        bool IsPublishConfigured();
        bool IsPreviewConfigured();
        bool IsRootDictionariesDisabled();
        bool IsMasterContentDisabled();
    }
}