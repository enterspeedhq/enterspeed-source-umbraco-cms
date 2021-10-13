using Enterspeed.Source.UmbracoCms.V9.Models.Configuration;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public interface IEnterspeedConfigurationService
    {
        void Save(EnterspeedUmbracoConfiguration configuration);
        EnterspeedUmbracoConfiguration GetConfiguration();
    }
}