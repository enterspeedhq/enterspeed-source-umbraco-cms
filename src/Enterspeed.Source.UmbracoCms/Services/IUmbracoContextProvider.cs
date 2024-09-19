using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IUmbracoContextProvider
    {
        IUmbracoContext GetContext();
    }
}
