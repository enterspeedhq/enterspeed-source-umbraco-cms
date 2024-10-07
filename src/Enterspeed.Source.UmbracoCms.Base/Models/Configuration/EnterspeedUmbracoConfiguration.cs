using Enterspeed.Source.Sdk.Configuration;

namespace Enterspeed.Source.UmbracoCms.Base.Models.Configuration
{
    public class EnterspeedUmbracoConfiguration : EnterspeedConfiguration
    {
        public string MediaDomain { get; set; }
        public bool IsConfigured { get; set; }
        public bool ConfiguredFromSettingsFile { get; set; }
        public string PreviewApiKey { get; set; }
        public bool RootDictionariesDisabled { get; set; }
        public bool RunJobsOnAllServerRoles { get; set; }
        public bool EnableMasterContent { get; set; }
    }
}