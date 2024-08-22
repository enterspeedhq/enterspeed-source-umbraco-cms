using Umbraco.Cms.Web.Common.Routing;

namespace Enterspeed.Source.UmbracoCms.Configuration
{
    public class EnterspeedVersionedRouteAttribute : BackOfficeRouteAttribute
    {
        public EnterspeedVersionedRouteAttribute(string template)
                    : base($"enterspeed/api/v{{version:apiVersion}}/{template.TrimStart('/')}")
        {
        }
    }
}
