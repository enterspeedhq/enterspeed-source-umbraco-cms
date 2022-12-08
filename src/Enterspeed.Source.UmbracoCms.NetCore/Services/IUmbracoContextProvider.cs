using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services
{
    public interface IUmbracoContextProvider
    {
        IUmbracoContext GetContext();
    }
}
