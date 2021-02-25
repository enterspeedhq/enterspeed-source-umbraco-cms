using System;
using AutoFixture;
using Enterspeed.Source.UmbracoCms.V8.Models.Configuration;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Enterspeed.Source.UmbracoCms.V8.Tests.Setup;
using NSubstitute;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Xunit;

namespace Enterspeed.Source.UmbracoCms.V8.Tests.Providers
{
    public class UmbracoMediaUrlProviderTests
    {
        public class UmbracoMediaUrlProviderTestFixture : UmbracoTestFixture
        {
            public IEnterspeedConfigurationService ConfigurationService { get; set; }

            public UmbracoMediaUrlProviderTestFixture()
            {
                ConfigurationService = this.Freeze<IEnterspeedConfigurationService>();
            }
        }

        [Fact]
        public void GetUrl_AbsoluteDomain_Equal()
        {
            var fixture = new UmbracoMediaUrlProviderTestFixture();

            fixture.ConfigurationService
                .GetConfiguration()
                .Returns(new EnterspeedUmbracoConfiguration()
                {
                    MediaDomain = "https://enterspeed.com"
                });

            fixture.MediaUrlProvider.GetMediaUrl(
                Arg.Any<UmbracoContext>(),
                Arg.Any<IPublishedContent>(),
                Arg.Any<string>(),
                Arg.Any<UrlMode>(),
                Arg.Any<string>(),
                Arg.Any<Uri>())
                .Returns(new UrlInfo("/media/image.jpg", true, string.Empty));

            using (fixture.EnsureUmbracoContext())
            {
                var sut = fixture.Create<UmbracoMediaUrlProvider>();

                var url = sut.GetUrl(fixture.GetMedia());
                Assert.Equal("https://enterspeed.com/media/image.jpg", url);
            }
        }

        [Fact]
        public void GetUrl_NoDomainSet_Equal()
        {
            var fixture = new UmbracoMediaUrlProviderTestFixture();

            fixture.ConfigurationService
                .GetConfiguration()
                .Returns(new EnterspeedUmbracoConfiguration()
                {
                    MediaDomain = null
                });

            fixture.MediaUrlProvider.GetMediaUrl(
                    Arg.Any<UmbracoContext>(),
                    Arg.Any<IPublishedContent>(),
                    Arg.Any<string>(),
                    Arg.Any<UrlMode>(),
                    Arg.Any<string>(),
                    Arg.Any<Uri>())
                .Returns(new UrlInfo("/media/image.jpg", true, string.Empty));

            using (fixture.EnsureUmbracoContext())
            {
                var sut = fixture.Create<UmbracoMediaUrlProvider>();

                var url = sut.GetUrl(fixture.GetMedia());
                Assert.Equal("/media/image.jpg", url);
            }
        }
    }
}