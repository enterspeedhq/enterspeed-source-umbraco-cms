using Enterspeed.Source.Sdk.Configuration;

namespace Enterspeed.Source.UmbracoCms.Models.Configuration
{
    public class EnterspeedUmbracoConfiguration : EnterspeedConfiguration
    {
        public string MediaDomain { get; set; }
        public bool IsConfigured { get; set; }
        public string PreviewApiKey { get; set; }
    }
}