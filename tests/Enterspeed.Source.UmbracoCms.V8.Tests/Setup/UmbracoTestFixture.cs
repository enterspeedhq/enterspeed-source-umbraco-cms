using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;

namespace Enterspeed.Source.UmbracoCms.V8.Tests.Setup
{
    public class UmbracoTestFixture : Fixture
    {
        public IUmbracoContextAccessor ContextAccessor { get; set; }
        public UmbracoContextFactory ContextFactory { get; set; }
        public IMediaUrlProvider MediaUrlProvider { get; set; }
        public IUrlProvider UrlProvider { get; set; }
        public IPublishedSnapshotService PublishedSnapshotService { get; set; }
        public IVariationContextAccessor VariationContextAccessor { get; set; }
        public IDefaultCultureAccessor DefaultCultureAccessor { get; set; }
        public IGlobalSettings GlobalSettings { get; set; }
        public IUserService UserService { get; set; }
        public UmbracoSettingsSection UmbracoSettingsSection { get; set; }
        public SimpleWorkerRequest SimpleWorkerRequest { get; set; }

        public UmbracoTestFixture()
        {
            Customize(new AutoNSubstituteCustomization());

            ContextAccessor = this.Freeze<IUmbracoContextAccessor>();
            MediaUrlProvider = this.Freeze<IMediaUrlProvider>();
            UrlProvider = this.Freeze<IUrlProvider>();
            PublishedSnapshotService = this.Freeze<IPublishedSnapshotService>();
            VariationContextAccessor = this.Freeze<IVariationContextAccessor>();
            DefaultCultureAccessor = this.Freeze<IDefaultCultureAccessor>();
            GlobalSettings = this.Freeze<IGlobalSettings>();
            UserService = this.Freeze<IUserService>();
            UmbracoSettingsSection = this.Freeze<UmbracoSettingsSection>();

            SimpleWorkerRequest = new SimpleWorkerRequest("/", @"c:\", "null.aspx", string.Empty, TextWriter.Null);

            ContextFactory = new UmbracoContextFactory(
                ContextAccessor,
                PublishedSnapshotService,
                VariationContextAccessor,
                DefaultCultureAccessor,
                UmbracoSettingsSection,
                GlobalSettings,
                new UrlProviderCollection(new[] { UrlProvider }),
                new MediaUrlProviderCollection(new[] { MediaUrlProvider }),
                UserService
            );
        }

        public UmbracoContextReference EnsureUmbracoContext()
        {
            var context = ContextFactory.EnsureUmbracoContext(new HttpContextWrapper(new HttpContext(SimpleWorkerRequest)));

            ContextAccessor
                .UmbracoContext
                .Returns(context.UmbracoContext);

            Current.UmbracoContextAccessor = ContextAccessor;

            return context;
        }

        public IPublishedContent GetMedia(Action<IPublishedContent> func = null)
        {
            var media = this.Freeze<IPublishedContent>();

            media.ItemType
                .Returns(PublishedItemType.Media);
            media.ContentType.ItemType
                .Returns(PublishedItemType.Media);

            func?.Invoke(media);

            return media;
        }
    }
}