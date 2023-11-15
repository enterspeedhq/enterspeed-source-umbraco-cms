using System;
using System.Collections.Generic;
using System.Web.Helpers;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Exceptions;
using Enterspeed.Source.UmbracoCms.V8.Factories;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Models.Api;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Routing;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers.PreviewContent
{
    public class EnterspeedPreviewContentPublishJobHandler : IEnterspeedJobHandler
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IUmbracoRedirectsService _redirectsService;
        private readonly IEnterspeedGuardService _enterspeedGuardService;
        private readonly IUrlFactory _urlFactory;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;
        private readonly IPublishedRouter _publishedRouter;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public EnterspeedPreviewContentPublishJobHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedPropertyService enterspeedPropertyService,
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IUmbracoRedirectsService redirectsService,
            IEnterspeedGuardService enterspeedGuardService,
            IUrlFactory urlFactory,
            IEnterspeedConnectionProvider enterspeedConnectionProvider,
            IPublishedRouter publishedRouter,
            IVariationContextAccessor variationContextAccessor)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedPropertyService = enterspeedPropertyService;
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _redirectsService = redirectsService;
            _enterspeedGuardService = enterspeedGuardService;
            _urlFactory = urlFactory;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
            _publishedRouter = publishedRouter;
            _variationContextAccessor = variationContextAccessor;
        }

        public virtual bool CanHandle(EnterspeedJob job)
        {
            return _enterspeedConnectionProvider.GetConnection(ConnectionType.Preview) != null
                   && job.EntityType == EnterspeedJobEntityType.Content
                   && job.JobType == EnterspeedJobType.Publish
                   && job.ContentState == EnterspeedContentState.Preview;
        }

        public virtual void Handle(EnterspeedJob job)
        {
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                if (!string.IsNullOrEmpty(job.Culture))
                {
                    _variationContextAccessor.VariationContext = new VariationContext(job.Culture);
                }

                var content = GetContent(job, context);
                if (!CanIngest(content, job))
                {
                    return;
                }

                Current.UmbracoContext.PublishedRequest = _publishedRouter.CreateRequest(context.UmbracoContext);
                Current.UmbracoContext.PublishedRequest.PublishedContent = content;
                var umbracoData = CreateUmbracoContentEntity(content, job);
                Ingest(umbracoData, job);
            }
        }

        protected virtual UmbracoContentEntity CreateUmbracoContentEntity(IPublishedContent content, EnterspeedJob job)
        {
            try
            {
                var redirects = _redirectsService.GetRedirects(content.Key, job.Culture);
                return new UmbracoContentEntity(
                    content, _enterspeedPropertyService, _entityIdentityService, _urlFactory, redirects, job.Culture);
            }
            catch (Exception e)
            {
                throw new JobHandlingException(
                    $"Failed creating entity ({job.EntityId}/{job.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}");
            }
        }

        protected virtual bool CanIngest(IPublishedContent content, EnterspeedJob job)
        {
            // Check if any of guards are against it
            return _enterspeedGuardService.CanIngest(content, job.Culture);
        }

        protected virtual IPublishedContent GetContent(EnterspeedJob job, UmbracoContextReference context)
        {
            var isContentId = int.TryParse(job.EntityId, out var contentId);
            var content = isContentId ? context.UmbracoContext.Content.GetById(true, contentId) : null;
            if (content == null)
            {
                throw new JobHandlingException($"Content with id {job.EntityId} not in cache");
            }

            return content;
        }

        protected virtual void Ingest(IEnterspeedEntity<IDictionary<string, IEnterspeedProperty>> umbracoData, EnterspeedJob job)
        {
            var ingestResponse = _enterspeedIngestService.Save(
                umbracoData, _enterspeedConnectionProvider.GetConnection(ConnectionType.Preview));
            if (!ingestResponse.Success)
            {
                var message = Json.Encode(new ErrorResponse(ingestResponse));
                throw new JobHandlingException($"Failed ingesting entity ({job.EntityId}/{job.Culture}). Message: {message}");
            }
        }
    }
}