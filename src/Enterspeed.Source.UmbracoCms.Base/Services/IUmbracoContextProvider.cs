using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public interface IUmbracoContextProvider
    {
        IUmbracoContext GetContext();
    }
}
