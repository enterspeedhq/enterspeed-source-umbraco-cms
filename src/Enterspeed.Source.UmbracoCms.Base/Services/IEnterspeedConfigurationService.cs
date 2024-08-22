using Enterspeed.Source.UmbracoCms.Base.Models.Configuration;

namespace Enterspeed.Source.UmbracoCms.Base.Services
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