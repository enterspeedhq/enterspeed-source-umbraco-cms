using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Enterspeed.Source.UmbracoCms.V8.Tests.Setup;
using NSubstitute;
using Umbraco.Web.Routing;
using Xunit;

namespace Enterspeed.Source.UmbracoCms.V8.Tests.Services
{
    public class UmbracoUrlServiceTests
    {
        public class UmbracoUrlServiceTestFixture : UmbracoTestFixture
        {
            public IUmbracoContextProvider UmbracoContextProvider { get; set; }
            public UmbracoUrlServiceTestFixture()
            {
                UmbracoContextProvider = this.Freeze<IUmbracoContextProvider>();

                GlobalSettings.Path.Returns("/umbraco");
            }
        }

        [Fact]
        public void GetIdFromIdUrl_NoUrl_Equal()
        {
            var fixture = new UmbracoUrlServiceTestFixture();

            var sut = fixture.Create<UmbracoUrlService>();

            var result = sut.GetIdFromIdUrl(string.Empty);

            Assert.Equal(0, result);
        }

        [Fact]
        public void GetIdFromIdUrl_NoIdInUrl_Equal()
        {
            var fixture = new UmbracoUrlServiceTestFixture();

            var sut = fixture.Create<UmbracoUrlService>();

            var result = sut.GetIdFromIdUrl("/url/with-no-id");

            Assert.Equal(0, result);
        }

        [Fact]
        public void GetIdFromIdUrl_IdInUrl_Equal()
        {
            var fixture = new UmbracoUrlServiceTestFixture();

            var sut = fixture.Create<UmbracoUrlService>();

            var result = sut.GetIdFromIdUrl("1234/url/with-id");

            Assert.Equal(1234, result);
        }

        [Fact]
        public void GetIdFromIdUrl_MultipleIntegersInUrl_Equal()
        {
            var fixture = new UmbracoUrlServiceTestFixture();

            var sut = fixture.Create<UmbracoUrlService>();

            var result = sut.GetIdFromIdUrl("1234/url/with-id/5678");

            Assert.Equal(1234, result);
        }

        [Fact]
        public void GetUrlFromIdUrl_NoDomains_Equal()
        {
            var fixture = new UmbracoUrlServiceTestFixture();

            using (var umbracoContextReference = fixture.EnsureUmbracoContext())
            {
                fixture.UmbracoContextProvider
                    .GetContext()
                    .Returns(umbracoContextReference.UmbracoContext);

                fixture.DomainCache
                    .GetAssigned(Arg.Any<int>(), Arg.Any<bool>())
                    .Returns(new List<Domain>());

                var sut = fixture.Create<UmbracoUrlService>();

                var result = sut.GetUrlFromIdUrl("1234/url/with-id", "en-US");

                Assert.Equal("/url/with-id", result);
            }
        }

        [Fact]
        public void GetUrlFromIdUrl_Domains_Equal()
        {
            var fixture = new UmbracoUrlServiceTestFixture();

            using (var umbracoContextReference = fixture.EnsureUmbracoContext())
            {
                fixture.UmbracoContextProvider
                    .GetContext()
                    .Returns(umbracoContextReference.UmbracoContext);

                umbracoContextReference.UmbracoContext.Domains
                    .GetAssigned(1234, false)
                    .Returns(new List<Domain>()
                    {
                        new Domain(5678, "https://enterspeed.com", 1234, new CultureInfo("en-US"), false)
                    });

                var sut = fixture.Create<UmbracoUrlService>();

                var result = sut.GetUrlFromIdUrl("1234/url/with-id", "en-US");

                Assert.Equal("https://enterspeed.com/url/with-id", result);
            }
        }

        [Fact]
        public void GetUrlFromIdUrl_Domains_Equal_No_Double_Slash_After_Domain()
        {
            var fixture = new UmbracoUrlServiceTestFixture();

            using (var umbracoContextReference = fixture.EnsureUmbracoContext())
            {
                fixture.UmbracoContextProvider
                    .GetContext()
                    .Returns(umbracoContextReference.UmbracoContext);

                umbracoContextReference.UmbracoContext.Domains
                    .GetAssigned(1234, false)
                    .Returns(new List<Domain>()
                    {
                        new Domain(5678, "https://enterspeed.com/", 1234, new CultureInfo("en-US"), false)
                    });

                var sut = fixture.Create<UmbracoUrlService>();

                var result = sut.GetUrlFromIdUrl("1234/url/with-id", "en-US");

                Assert.Equal("https://enterspeed.com/url/with-id", result);
            }
        }

        [Fact]
        public void GetUrlFromIdUrl_NoDomainsForCulture_Equal()
        {
            var fixture = new UmbracoUrlServiceTestFixture();

            using (var umbracoContextReference = fixture.EnsureUmbracoContext())
            {
                fixture.UmbracoContextProvider
                    .GetContext()
                    .Returns(umbracoContextReference.UmbracoContext);

                umbracoContextReference.UmbracoContext.Domains
                    .GetAssigned(1234, false)
                    .Returns(new List<Domain>()
                    {
                        new Domain(5678, "https://enterspeed.com", 1234, new CultureInfo("en-US"), false)
                    });

                var sut = fixture.Create<UmbracoUrlService>();

                var result = sut.GetUrlFromIdUrl("1234/url/with-id", "da-DK");

                Assert.Equal("/url/with-id", result);
            }
        }
    }
}