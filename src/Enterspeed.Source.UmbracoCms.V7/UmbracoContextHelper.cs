using System.IO;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Enterspeed.Source.UmbracoCms.V7
{
    public class UmbracoContextHelper
    {
        public static UmbracoHelper GetUmbracoHelper()
        {
            return new UmbracoHelper(GetUmbracoContext());
        }

        public static void EnsureUmbracoContext()
        {
            GetUmbracoContext();
        }

        public static UmbracoContext GetUmbracoContext()
        {
            var context = UmbracoContext.Current;
            if (context == null)
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current ?? new HttpContext(new SimpleWorkerRequest("temp.aspx", string.Empty, new StringWriter())));
                context = UmbracoContext.EnsureContext(
                    httpContext,
                    ApplicationContext.Current,
                    new WebSecurity(httpContext, ApplicationContext.Current),
                    UmbracoConfig.For.UmbracoSettings(),
                    UrlProviderResolver.Current.Providers,
                    true);
            }

            return context;
        }
    }
}
