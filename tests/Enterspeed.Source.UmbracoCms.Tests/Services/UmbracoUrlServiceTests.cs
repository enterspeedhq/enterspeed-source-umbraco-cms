using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Routing;
using Xunit;

namespace Enterspeed.Source.UmbracoCms.Tests.Services
{
    public class UmbracoUrlServiceTests
    {
        public class UmbracoUrlServiceTestsFixture : Fixture
        {
            public IUmbracoContextProvider UmbracoContextProvider { get; set; }
            public IOptions<GlobalSettings> GlobalSettings { get; set; }
            public IOptions<RequestHandlerSettings> RequestHandlerSettings { get; set; }

            public UmbracoUrlServiceTestsFixture()
            {
                Customize(new AutoNSubstituteCustomization());
                UmbracoContextProvider = this.Freeze<IUmbracoContextProvider>();
                GlobalSettings = this.Freeze<IOptions<GlobalSettings>>();
                RequestHandlerSettings = this.Freeze<IOptions<RequestHandlerSettings>>();
            }
        }

        [Fact]
        public void GetUrlFromIdUrl_Dont_Include_Double_Slash_After_Domain()
        {
            // Arrange
            var fixture = new UmbracoUrlServiceTestsFixture();
            var id = 1234;
            var idUrl = $"{id}/test";
            var culture = "en-US";
            var expected = "https://localhost/test";

            fixture.UmbracoContextProvider.GetContext().Domains.GetAssigned(id, false)
                .Returns(new List<Domain>
                {
                    new Domain(1, "https://localhost/", id, culture, false)
                });

            var sut = fixture.Create<UmbracoUrlService>();

            // Act
            var redirect = sut.GetUrlFromIdUrl(idUrl, culture);

            // Assert
            Assert.Equal(expected, redirect);
        }
    }
}