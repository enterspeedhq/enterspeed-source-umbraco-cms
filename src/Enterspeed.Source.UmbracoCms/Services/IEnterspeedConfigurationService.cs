using Enterspeed.Source.UmbracoCms.Models.Configuration;
using Umbraco.Cms.Core.Sync;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public interface IEnterspeedConfigurationService
    {
        void Save(EnterspeedUmbracoConfiguration configuration);
        EnterspeedUmbracoConfiguration GetConfiguration();
        bool IsPublishConfigured();
        bool IsPreviewConfigured();
        bool IsRootDictionariesDisabled();
    }
}