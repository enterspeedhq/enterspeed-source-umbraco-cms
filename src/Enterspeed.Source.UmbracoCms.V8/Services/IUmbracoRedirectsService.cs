using System;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IUmbracoRedirectsService
    {
        string[] GetRedirects(Guid contentKey, string culture);
    }
}