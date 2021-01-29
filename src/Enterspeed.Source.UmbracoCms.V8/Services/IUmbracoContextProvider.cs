using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IUmbracoContextProvider
    {
        UmbracoContext GetContext();
        void EnsureContext();
    }
}
