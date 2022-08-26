using System;

namespace Enterspeed.Source.UmbracoCms.V10.Services
{
    public interface IUmbracoRedirectsService
    {
        string[] GetRedirects(Guid contentKey, string culture);
    }
}