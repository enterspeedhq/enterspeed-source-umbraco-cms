#if UMBRACO_18_OR_GREATER
using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.NotificationHandlers.Umbraco18;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;
using Xunit;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Tests.NotificationHandlers
{
    public class EnterspeedElementCacheRefresherNotificationHandlerTests
    {
        private readonly IEnterspeedConfigurationService _configurationService = Substitute.For<IEnterspeedConfigurationService>();
        private readonly IEnterspeedJobRepository _jobRepository = Substitute.For<IEnterspeedJobRepository>();
        private readonly IEnterspeedJobsHandlingService _jobsHandlingService = Substitute.For<IEnterspeedJobsHandlingService>();
        private readonly IUmbracoContextFactory _umbracoContextFactory = Substitute.For<IUmbracoContextFactory>();
        private readonly IScopeProvider _scopeProvider = Substitute.For<IScopeProvider>();
        private readonly IRelationService _relationService = Substitute.For<IRelationService>();
        private readonly IEnterspeedJobFactory _jobFactory = Substitute.For<IEnterspeedJobFactory>();
        private readonly IAuditService _auditService = Substitute.For<IAuditService>();
        private readonly IUmbracoCultureProvider _cultureProvider = Substitute.For<IUmbracoCultureProvider>();
        private readonly IServerRoleAccessor _serverRoleAccessor = Substitute.For<IServerRoleAccessor>();
        private readonly IPublishedContentCache _contentCache = Substitute.For<IPublishedContentCache>();

        public EnterspeedElementCacheRefresherNotificationHandlerTests()
        {
            var umbracoContext = Substitute.For<IUmbracoContext>();
            umbracoContext.Content.Returns(_contentCache);
            _umbracoContextFactory.EnsureUmbracoContext().Returns(_ => new UmbracoContextReference(
                umbracoContext, false, Substitute.For<IUmbracoContextAccessor>()));

            // Keep EnqueueJobs on the repository-only path
            _jobsHandlingService.IsJobsProcessingEnabled().Returns(false);
        }

        private EnterspeedElementCacheRefresherNotificationHandler CreateSut()
        {
            return new EnterspeedElementCacheRefresherNotificationHandler(
                _configurationService,
                _jobRepository,
                _jobsHandlingService,
                _umbracoContextFactory,
                _scopeProvider,
                _relationService,
                _jobFactory,
                _auditService,
                _cultureProvider,
                _serverRoleAccessor,
                Substitute.For<ILogger<EnterspeedElementCacheRefresherNotificationHandler>>());
        }

        private static ElementCacheRefresherNotification CreateNotification(params ElementCacheRefresher.JsonPayload[] payloads)
        {
            return new ElementCacheRefresherNotification(payloads, MessageType.RefreshByPayload);
        }

        private IPublishedContent CreateInvariantPublishedContent(string culture)
        {
            var contentType = Substitute.For<IPublishedContentType>();
            contentType.Variations.Returns(ContentVariation.Nothing);

            var content = Substitute.For<IPublishedContent>();
            content.ContentType.Returns(contentType);
            _cultureProvider.GetCultureForNonCultureVariant(content).Returns(culture);
            return content;
        }

        private static IRelation Relation(int parentId, int childId)
        {
            return new Relation(parentId, childId, Substitute.For<IRelationType>());
        }

        [Fact]
        public void Handle_EnqueuesOnePublishJobPerReferencingDocument_Deduplicated()
        {
            _configurationService.IsPublishConfigured().Returns(true);

            // Elements 100 and 101 are both referenced by document 200;
            // element 100 is also referenced by document 201, which is unpublished
            _relationService.GetByChildId(100, Constants.Conventions.RelationTypes.RelatedElementAlias)
                .Returns(new[] { Relation(200, 100), Relation(201, 100) });
            _relationService.GetByChildId(101, Constants.Conventions.RelationTypes.RelatedElementAlias)
                .Returns(new[] { Relation(200, 101) });

            var publishedPage = CreateInvariantPublishedContent("en-us");
            _contentCache.GetById(200).Returns(publishedPage);
            _contentCache.GetById(201).Returns((IPublishedContent)null);
            _contentCache.GetById(true, 201).Returns((IPublishedContent)null);

            var job = new EnterspeedJob();
            _jobFactory.GetPublishJob(publishedPage, "en-us", EnterspeedContentState.Publish).Returns(job);

            CreateSut().Handle(CreateNotification(
                new ElementCacheRefresher.JsonPayload(100, Guid.NewGuid(), TreeChangeTypes.RefreshNode) { PublishedCultures = new[] { "*" } },
                new ElementCacheRefresher.JsonPayload(101, Guid.NewGuid(), TreeChangeTypes.RefreshNode) { PublishedCultures = new[] { "*" } }));

            _jobRepository.Received(1).Save(Arg.Is<IList<EnterspeedJob>>(jobs => jobs.Count == 1 && jobs[0] == job));
        }

        [Fact]
        public void Handle_DraftOnlySave_DoesNotEnqueuePublishJobs()
        {
            // A RefreshNode payload without culture information is a draft-only save;
            // the publish source must stay untouched (test plan C3)
            _configurationService.IsPublishConfigured().Returns(true);
            _configurationService.IsPreviewConfigured().Returns(true);

            _relationService.GetByChildId(100, Constants.Conventions.RelationTypes.RelatedElementAlias)
                .Returns(new[] { Relation(200, 100) });

            var publishedPage = CreateInvariantPublishedContent("en-us");
            _contentCache.GetById(200).Returns(publishedPage);
            var savedPage = CreateInvariantPublishedContent("en-us");
            _contentCache.GetById(true, 200).Returns(savedPage);

            var previewJob = new EnterspeedJob();
            _jobFactory.GetPublishJob(savedPage, "en-us", EnterspeedContentState.Preview).Returns(previewJob);

            CreateSut().Handle(CreateNotification(
                new ElementCacheRefresher.JsonPayload(100, Guid.NewGuid(), TreeChangeTypes.RefreshNode)));

            _jobFactory.DidNotReceive().GetPublishJob(Arg.Any<IPublishedContent>(), Arg.Any<string>(), EnterspeedContentState.Publish);
            _jobRepository.Received(1).Save(Arg.Is<IList<EnterspeedJob>>(jobs => jobs.Count == 1 && jobs[0] == previewJob));
        }

        [Fact]
        public void Handle_ElementRemoved_EnqueuesPublishJobsDespiteMissingCultureInfo()
        {
            // Deletes carry no culture info but must reingest referencing pages so the
            // removed element drops out of their payloads (test plan C5)
            _configurationService.IsPublishConfigured().Returns(true);

            _relationService.GetByChildId(100, Constants.Conventions.RelationTypes.RelatedElementAlias)
                .Returns(new[] { Relation(200, 100) });

            var publishedPage = CreateInvariantPublishedContent("en-us");
            _contentCache.GetById(200).Returns(publishedPage);

            var job = new EnterspeedJob();
            _jobFactory.GetPublishJob(publishedPage, "en-us", EnterspeedContentState.Publish).Returns(job);

            CreateSut().Handle(CreateNotification(
                new ElementCacheRefresher.JsonPayload(100, Guid.NewGuid(), TreeChangeTypes.Remove)));

            _jobRepository.Received(1).Save(Arg.Is<IList<EnterspeedJob>>(jobs => jobs.Count == 1 && jobs[0] == job));
        }

        [Fact]
        public void Handle_RefreshAll_FansOutToAllElementReferencingDocuments()
        {
            // RefreshAll payloads carry no element id (Umbraco publishes them with id -1/0),
            // so the fan-out must resolve every umbElement relation instead
            _configurationService.IsPublishConfigured().Returns(true);

            _relationService.GetByRelationTypeAlias(Constants.Conventions.RelationTypes.RelatedElementAlias)
                .Returns(new[] { Relation(200, 100), Relation(201, 101) });

            var page200 = CreateInvariantPublishedContent("en-us");
            var page201 = CreateInvariantPublishedContent("en-us");
            _contentCache.GetById(200).Returns(page200);
            _contentCache.GetById(201).Returns(page201);

            var job200 = new EnterspeedJob();
            var job201 = new EnterspeedJob();
            _jobFactory.GetPublishJob(page200, "en-us", EnterspeedContentState.Publish).Returns(job200);
            _jobFactory.GetPublishJob(page201, "en-us", EnterspeedContentState.Publish).Returns(job201);

            CreateSut().Handle(CreateNotification(
                new ElementCacheRefresher.JsonPayload(0, Guid.Empty, TreeChangeTypes.RefreshAll)));

            _relationService.DidNotReceiveWithAnyArgs().GetByChildId(0, null);
            _jobRepository.Received(1).Save(Arg.Is<IList<EnterspeedJob>>(jobs => jobs.Count == 2));
        }

        [Fact]
        public void Handle_VariantPage_OnlyAffectedCulturesAreReingested()
        {
            _configurationService.IsPublishConfigured().Returns(true);

            _relationService.GetByChildId(100, Constants.Conventions.RelationTypes.RelatedElementAlias)
                .Returns(new[] { Relation(200, 100) });

            var contentType = Substitute.For<IPublishedContentType>();
            contentType.Variations.Returns(ContentVariation.Culture);
            var publishedPage = Substitute.For<IPublishedContent>();
            publishedPage.ContentType.Returns(contentType);
            _cultureProvider.GetCulturesForCultureVariant(publishedPage).Returns(new List<string> { "en-us", "da-dk" });
            _contentCache.GetById(200).Returns(publishedPage);

            var job = new EnterspeedJob();
            _jobFactory.GetPublishJob(publishedPage, "da-dk", EnterspeedContentState.Publish).Returns(job);

            var payload = new ElementCacheRefresher.JsonPayload(100, Guid.NewGuid(), TreeChangeTypes.RefreshNode)
            {
                PublishedCultures = new[] { "da-dk" },
            };

            CreateSut().Handle(CreateNotification(payload));

            _jobFactory.Received(1).GetPublishJob(publishedPage, "da-dk", EnterspeedContentState.Publish);
            _jobFactory.DidNotReceive().GetPublishJob(publishedPage, "en-us", EnterspeedContentState.Publish);
            _jobRepository.Received(1).Save(Arg.Is<IList<EnterspeedJob>>(jobs => jobs.Count == 1 && jobs[0] == job));
        }

        [Fact]
        public void Handle_PreviewConfigured_EnqueuesPreviewJobForSavedPage()
        {
            _configurationService.IsPreviewConfigured().Returns(true);

            _relationService.GetByChildId(100, Constants.Conventions.RelationTypes.RelatedElementAlias)
                .Returns(new[] { Relation(200, 100) });

            var savedPage = CreateInvariantPublishedContent("en-us");
            _contentCache.GetById(true, 200).Returns(savedPage);

            var job = new EnterspeedJob();
            _jobFactory.GetPublishJob(savedPage, "en-us", EnterspeedContentState.Preview).Returns(job);

            CreateSut().Handle(CreateNotification(
                new ElementCacheRefresher.JsonPayload(100, Guid.NewGuid(), TreeChangeTypes.RefreshNode)));

            _jobRepository.Received(1).Save(Arg.Is<IList<EnterspeedJob>>(jobs => jobs.Count == 1 && jobs[0] == job));
        }

        [Fact]
        public void Handle_NoReferencingDocuments_EnqueuesNothing()
        {
            _configurationService.IsPublishConfigured().Returns(true);
            _relationService.GetByChildId(Arg.Any<int>(), Arg.Any<string>()).Returns(Array.Empty<IRelation>());

            CreateSut().Handle(CreateNotification(
                new ElementCacheRefresher.JsonPayload(100, Guid.NewGuid(), TreeChangeTypes.RefreshNode)));

            _jobRepository.DidNotReceive().Save(Arg.Any<IList<EnterspeedJob>>());
        }

        [Fact]
        public void Handle_NothingConfigured_DoesNotTouchRelations()
        {
            CreateSut().Handle(CreateNotification(
                new ElementCacheRefresher.JsonPayload(100, Guid.NewGuid(), TreeChangeTypes.RefreshNode)));

            _relationService.DidNotReceiveWithAnyArgs().GetByChildId(0, null);
            _jobRepository.DidNotReceive().Save(Arg.Any<IList<EnterspeedJob>>());
        }

        [Theory]
        [InlineData(new string[] { }, new[] { "en-us", "da-dk" })] // no culture info -> all page cultures
        [InlineData(new[] { "da-dk" }, new[] { "da-dk" })] // intersection
        [InlineData(new[] { "de-de" }, new[] { "en-us", "da-dk" })] // disjoint -> fall back to all
        public void FilterAffectedCultures_ScopesToAffectedCulturesWithSafeFallback(string[] affected, string[] expected)
        {
            var result = EnterspeedElementCacheRefresherNotificationHandler.FilterAffectedCultures(
                new List<string> { "en-us", "da-dk" },
                new HashSet<string>(affected));

            Assert.Equal(expected, result);
        }
    }
}
#endif
