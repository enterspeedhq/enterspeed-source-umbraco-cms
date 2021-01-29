using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class UmbracoContextProvider : IUmbracoContextProvider
    {
        private IUmbracoContextFactory _umbracoContextFactory;
        private IUmbracoContextFactory UmbracoContextFactory => _umbracoContextFactory ?? (_umbracoContextFactory = Current.Factory.GetInstance<IUmbracoContextFactory>());

        public UmbracoContext GetContext()
        {
            EnsureContext();

            return Current.UmbracoContext;
        }

        public void EnsureContext()
        {
            var umbracoContext = Current.UmbracoContext;
            if (umbracoContext == null || umbracoContext.Disposed)
            {
                UmbracoContextFactory.EnsureUmbracoContext();
            }
        }
    }
}
