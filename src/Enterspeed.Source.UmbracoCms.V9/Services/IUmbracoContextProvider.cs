using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public interface IUmbracoContextProvider
    {
        IUmbracoContext GetContext();
    }
}
