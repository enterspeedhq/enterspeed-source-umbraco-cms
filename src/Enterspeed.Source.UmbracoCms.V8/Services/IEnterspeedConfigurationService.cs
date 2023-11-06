using Enterspeed.Source.UmbracoCms.V8.Models.Configuration;
using Umbraco.Core.Sync;

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