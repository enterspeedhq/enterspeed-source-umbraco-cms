﻿using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.NetCore.Providers
{
    public interface IUmbracoMediaUrlProvider
    {
        string GetUrl(IPublishedContent media);
    }
}
