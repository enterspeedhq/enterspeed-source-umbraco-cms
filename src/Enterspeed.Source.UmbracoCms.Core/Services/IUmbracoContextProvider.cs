using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.Core.Services
{
    public interface IUmbracoContextProvider
    {
        IUmbracoContext GetContext();
    }
}
