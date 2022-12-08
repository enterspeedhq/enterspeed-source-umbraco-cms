using System;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services
{
    public interface IUmbracoRedirectsService
    {
        string[] GetRedirects(Guid contentKey, string culture);
    }
}