using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public class UmbracoContextProvider : IUmbracoContextProvider
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public UmbracoContextProvider(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _umbracoContextFactory = umbracoContextFactory;
        }

        public IUmbracoContext GetContext()
        {
            var hasUmbracoContext = _umbracoContextAccessor.TryGetUmbracoContext(out var context);
            if (hasUmbracoContext && context != null)
            {
                return context;
            }

            var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
            return umbracoContextReference.UmbracoContext;
        }
    }
}