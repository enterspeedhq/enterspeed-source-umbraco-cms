using System;
using System.Text.Json;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Exceptions;
using Enterspeed.Source.UmbracoCms.V9.Factories;
using Enterspeed.Source.UmbracoCms.V9.Models;
using Enterspeed.Source.UmbracoCms.V9.Providers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers.Content
{
    public class EnterspeedContentPublishJobHandler : IEnterspeedJobHandler
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IUmbracoRedirectsService _redirectsService;
        private readonly IEnterspeedGuardService _enterspeedGuardService;
        private readonly IUrlFactory _urlFactory;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;

        public EnterspeedContentPublishJobHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedPropertyService enterspeedPropertyService,
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IUmbracoRedirectsService redirectsService,
            IEnterspeedGuardService enterspeedGuardService,
            IUrlFactory urlFactory,
            IEnterspeedConnectionProvider enterspeedConnectionProvider)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedPropertyService = enterspeedPropertyService;
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _redirectsService = redirectsService;
            _enterspeedGuardService = enterspeedGuardService;
            _urlFactory = urlFactory;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
        }

        public virtual bool CanHandle(EnterspeedJob job)
        {
            return 
                   _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish) != null
                   && job.EntityType == EnterspeedJobEntityType.Content 
                   && job.JobType == EnterspeedJobType.Publish 
                   && job.ContentState == EnterspeedContentState.Publish;
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
                    content, _enterspeedPropertyService, _entityIdentityService, redirects,
                    _urlFactory, job.Culture);
            }
            catch (Exception e)
            {
                throw new JobHandlingException(
                    $"Failed creating entity ({job.EntityId}/{job.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}");
            }
        }
        
        protected virtual void Ingest(IEnterspeedEntity umbracoData, EnterspeedJob job)
        {
            var ingestResponse = _enterspeedIngestService.Save(umbracoData, _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish));
            if (!ingestResponse.Success)
            {
                var message = ingestResponse.Errors != null
                    ? JsonSerializer.Serialize(ingestResponse.Errors)
                    : ingestResponse.Message;
                throw new JobHandlingException(
                    $"Failed ingesting entity ({job.EntityId}/{job.Culture}). Message: {message}");
            }
        }
    }
}