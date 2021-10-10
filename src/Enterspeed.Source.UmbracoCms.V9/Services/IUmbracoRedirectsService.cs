using System;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public interface IUmbracoRedirectsService
    {
        string[] GetRedirects(Guid contentKey, string culture);
    }
}