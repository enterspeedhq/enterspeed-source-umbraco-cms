using System;

namespace Enterspeed.Source.UmbracoCms.Core.Services
{
    public interface IUmbracoRedirectsService
    {
        string[] GetRedirects(Guid contentKey, string culture);
    }
}