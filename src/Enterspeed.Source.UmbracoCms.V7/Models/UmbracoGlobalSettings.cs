using System.Configuration;

namespace Enterspeed.Source.UmbracoCms.V7.Models
{
    public class UmbracoGlobalSettings
    {
        public bool UseHttps
        {
            get
            {
                var value = ConfigurationManager.AppSettings["umbracoUseSSL"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    return false;
                }

                if (bool.TryParse(value, out var result))
                {
                    return result;
                }

                return false;
            }
        }
    }
}
