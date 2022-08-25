using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.V10.Services
{
    public interface IUmbracoContextProvider
    {
        IUmbracoContext GetContext();
    }
}
