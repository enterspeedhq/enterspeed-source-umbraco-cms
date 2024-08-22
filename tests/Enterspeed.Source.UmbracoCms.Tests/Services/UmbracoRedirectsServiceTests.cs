using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Enterspeed.Source.UmbracoCms.Base.Services;
using NSubstitute;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Xunit;

namespace Enterspeed.Source.UmbracoCms.Tests.Services
{
    public class UmbracoRedirectsServiceTests
    {
        public class UmbracoRedirectsServiceTestFixture : Fixture
        {
            public IRedirectUrlService RedirectUrlService { get; set; }
            public IUmbracoUrlService UmbracoUrlService { get; set; }

            public UmbracoRedirectsServiceTestFixture()
            {
                Customize(new AutoNSubstituteCustomization());
                RedirectUrlService = this.Freeze<IRedirectUrlService>();
                UmbracoUrlService = this.Freeze<IUmbracoUrlService>();
            }
        }

        [Fact]
        public void GetRedirects_NoRedirectsFound_Empty()
        {
            // Arrange
            var fixture = new UmbracoRedirectsServiceTestFixture();
            var guid = Guid.NewGuid();

            fixture.RedirectUrlService
                .GetContentRedirectUrls(guid)
                .Returns(new List<IRedirectUrl>());

            var sut = fixture.Create<UmbracoRedirectsService>();

            // Act & Assert
            Assert.Empty(sut.GetRedirects(guid, "en-US"));
        }

        [Fact]
        public void GetRedirects_NoRedirectsFoundForCulture_Empty()
        {
            // Arrange
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

            // Act & Assert
            Assert.Empty(sut.GetRedirects(guid, "en-US"));
        }

        [Fact]
        public void GetRedirects_RedirectsFoundForCulture_Equal()
        {
            // Arrange
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

            // Act
            var redirect = sut.GetRedirects(guid, "en-US")[0];

            // Assert
            Assert.Equal(expected, redirect);
        }
    }
}