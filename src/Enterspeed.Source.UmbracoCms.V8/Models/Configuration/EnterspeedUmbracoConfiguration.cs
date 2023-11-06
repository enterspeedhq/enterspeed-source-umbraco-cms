using Enterspeed.Source.Sdk.Configuration;

namespace Enterspeed.Source.UmbracoCms.V8.Models.Configuration
{
    public class EnterspeedUmbracoConfiguration : EnterspeedConfiguration
    {
        public string MediaDomain { get; set; }
        public bool IsConfigured { get; set; }
        public string PreviewApiKey { get; set; }
        public bool RunJobsOnAllServerRoles { get; set; }
    }
}