using System;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public interface IUmbracoRedirectsService
    {
        string[] GetRedirects(Guid contentKey, string culture);
    }
}