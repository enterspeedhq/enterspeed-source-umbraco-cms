using System;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Exceptions;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Routing;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
{
    public class EnterspeedPublishedContentJobHandler : IEnterspeedJobHandler
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IUmbracoRedirectsService _redirectsService;
        private readonly IEnterspeedGuardService _enterspeedGuardService;
        private readonly IPublishedRouter _publishedRouter;

        public EnterspeedPublishedContentJobHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedPropertyService enterspeedPropertyService,
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IUmbracoRedirectsService redirectsService,
            IEnterspeedGuardService enterspeedGuardService,
            IPublishedRouter publishedRouter)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedPropertyService = enterspeedPropertyService;
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _redirectsService = redirectsService;
            _enterspeedGuardService = enterspeedGuardService;
            _publishedRouter = publishedRouter;
        }

        public virtual bool CanHandle(EnterspeedJob job)
        {
            return job.EntityType == EnterspeedJobEntityType.Content && job.JobType == EnterspeedJobType.Publish;
        }

        public virtual void Handle(EnterspeedJob job)
        {
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var content = GetContent(job, context);
                if (!CanIngest(content, job))
                {
                    return;
                }

                var umbracoData = CreateUmbracoContentEntity(content, job);
                Current.UmbracoContext.PublishedRequest = _publishedRouter.CreateRequest(context.UmbracoContext);
                Current.UmbracoContext.PublishedRequest.PublishedContent = content;
                Ingest(umbracoData, job);
            }
        }

        protected virtual IPublishedContent GetContent(EnterspeedJob job, UmbracoContextReference context)
        {
            var isContentId = int.TryParse(job.EntityId, out var contentId);
            var content = isContentId ? context.UmbracoContext.Content.GetById(contentId) : null;
            if (content == null)
            {
                throw new JobHandlingException($"Content with id {job.EntityId} not in cache");
            }

            return content;
        }

        protected virtual bool CanIngest(IPublishedContent content, EnterspeedJob job)
        {
            // Check if any of guards are against it
            return _enterspeedGuardService.CanIngest(content, job.Culture);
        }

        protected virtual UmbracoContentEntity CreateUmbracoContentEntity(IPublishedContent content, EnterspeedJob job)
        {
            try
            {
                var redirects = _redirectsService.GetRedirects(content.Key, job.Culture);
                return new UmbracoContentEntity(
                    content, _enterspeedPropertyService, _entityIdentityService, redirects, job.Culture);
            }
            catch (Exception e)
            {
                throw new JobHandlingException(
                    $"Failed creating entity ({job.EntityId}/{job.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}");
            }
        }

        protected virtual void Ingest(IEnterspeedEntity umbracoData, EnterspeedJob job)
        {
            var ingestResponse = _enterspeedIngestService.Save(umbracoData);
            if (!ingestResponse.Success)
            {
                var message = ingestResponse.Exception != null
                    ? ingestResponse.Exception.Message
                    : ingestResponse.Message;
                throw new JobHandlingException(
                    $"Failed ingesting entity ({job.EntityId}/{job.Culture}). Message: {message}");
            }
        }
    }
}