using System;
using System.Collections.Generic;
using AutoFixture;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Enterspeed.Source.UmbracoCms.V8.Tests.Setup;
using NSubstitute;
using Umbraco.Core.Models;
using Xunit;

namespace Enterspeed.Source.UmbracoCms.V8.Tests.Services
{
    public class UmbracoRedirectsServiceTests
    {
        public class UmbracoRedirectsServiceTestFixture : UmbracoTestFixture
        {
            public IUmbracoUrlService UmbracoUrlService { get; set; }

            public UmbracoRedirectsServiceTestFixture()
            {
                UmbracoUrlService = this.Freeze<IUmbracoUrlService>();
            }
        }

        [Fact]
        public void GetRedirects_NoRedirectsFound_Empty()
        {
            var fixture = new UmbracoRedirectsServiceTestFixture();
            var guid = Guid.NewGuid();

            fixture.RedirectUrlService
                .GetContentRedirectUrls(guid)
                .Returns(new List<IRedirectUrl>());

            var sut = fixture.Create<UmbracoRedirectsService>();

            Assert.Empty(sut.GetRedirects(guid, "en-US"));
        }

        [Fact]
        public void GetRedirects_NoRedirectsFoundForCulture_Empty()
        {
            var fixture = new UmbracoRedirectsServiceTestFixture();
            var guid = Guid.NewGuid();

            fixture.RedirectUrlService
                .GetContentRedirectUrls(guid)
                .Returns(new List<IRedirectUrl>()
                {
                    new RedirectUrl()
                    {
                        ContentId = 1234,
                        ContentKey = guid,
                        Culture = "da-DK",
                        Url = "/da-dk/redirect"
                    }
                });

            var sut = fixture.Create<UmbracoRedirectsService>();

            Assert.Empty(sut.GetRedirects(guid, "en-US"));
        }

        [Fact]
        public void GetRedirects_RedirectsFoundForCulture_Equal()
        {
            var fixture = new UmbracoRedirectsServiceTestFixture();
            var guid = Guid.NewGuid();

            var expected = "/en-us/redirect";

            fixture.RedirectUrlService
                .GetContentRedirectUrls(guid)
                .Returns(new List<IRedirectUrl>()
                {
                    new RedirectUrl()
                    {
                        ContentId = 1234,
                        ContentKey = guid,
                        Culture = "en-US",
                        Url = expected
                    }
                });

            fixture.UmbracoUrlService
                .GetUrlFromIdUrl(Arg.Any<string>(), Arg.Any<string>())
                .Returns(expected);

            var sut = fixture.Create<UmbracoRedirectsService>();

            var redirect = sut.GetRedirects(guid, "en-US")[0];

            Assert.Equal(expected, redirect);
        }
    }
}