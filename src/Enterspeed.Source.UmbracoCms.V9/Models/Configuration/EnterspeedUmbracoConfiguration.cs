using Enterspeed.Source.Sdk.Configuration;

namespace Enterspeed.Source.UmbracoCms.V9.Models.Configuration
{
    public class EnterspeedUmbracoConfiguration : EnterspeedConfiguration
    {
        public string MediaDomain { get; set; }
        public bool IsConfigured { get; set; }
    }
}