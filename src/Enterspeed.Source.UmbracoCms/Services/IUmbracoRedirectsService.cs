using System;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IUmbracoRedirectsService
    {
        string[] GetRedirects(Guid contentKey, string culture);
    }
}