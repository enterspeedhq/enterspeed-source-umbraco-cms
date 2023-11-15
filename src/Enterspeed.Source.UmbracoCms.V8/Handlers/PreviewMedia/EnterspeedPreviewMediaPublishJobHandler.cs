using System;
using System.Collections.Generic;
using System.Web.Helpers;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Exceptions;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Models.Api;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers.PreviewMedia
{
    public class EnterspeedPreviewMediaPublishJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IEnterspeedGuardService _enterspeedGuardService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;
        private readonly IMediaService _mediaService;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public EnterspeedPreviewMediaPublishJobHandler(
            IEnterspeedPropertyService enterspeedPropertyService,
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IEnterspeedGuardService enterspeedGuardService,
            IEnterspeedConnectionProvider enterspeedConnectionProvider,
            IMediaService mediaService,
            IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _enterspeedPropertyService = enterspeedPropertyService;
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _enterspeedGuardService = enterspeedGuardService;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
            _mediaService = mediaService;
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        public virtual bool CanHandle(EnterspeedJob job)
        {
            return
                _enterspeedConnectionProvider.GetConnection(ConnectionType.Preview) != null
                && job.EntityType == EnterspeedJobEntityType.Media
                && job.ContentState == EnterspeedContentState.Preview
                && job.JobType == EnterspeedJobType.Publish;
        }

        public virtual void Handle(EnterspeedJob job)
        {
            var media = GetMedia(job);
            if (!CanIngest(media, job))
            {
                return;
            }

            var umbracoData = CreateUmbracoMediaEntity(media, job);
            Ingest(umbracoData, job);
        }

        protected virtual IMedia GetMedia(EnterspeedJob job)
        {
            var parsed = int.TryParse(job.EntityId, out var parsedId);
            var media = parsed ? _mediaService.GetById(parsedId) : null;

            if (media == null)
            {
                throw new JobHandlingException($"Media with id {job.EntityId} not in database");
            }

            return media;
        }

        protected virtual bool CanIngest(IMedia media, EnterspeedJob job)
        {
            // Check if any of guards are against it
            return _enterspeedGuardService.CanIngest(media, job.Culture);
        }

        protected virtual UmbracoMediaEntity CreateUmbracoMediaEntity(IMedia media, EnterspeedJob job)
        {
            try
            {
                return new UmbracoMediaEntity(media, _enterspeedPropertyService, _entityIdentityService, _enterspeedConfigurationService);
            }
            catch (Exception e)
            {
                throw new JobHandlingException(
                    $"Failed creating entity ({job.EntityId}). Message: {e.Message}. StackTrace: {e.StackTrace}");
            }
        }

        protected virtual void Ingest(IEnterspeedEntity<IDictionary<string, IEnterspeedProperty>> umbracoData, EnterspeedJob job)
        {
            var ingestResponse = _enterspeedIngestService.Save(umbracoData, _enterspeedConnectionProvider.GetConnection(ConnectionType.Preview));
            if (!ingestResponse.Success)
            {
                var message = Json.Encode(new ErrorResponse(ingestResponse));
                throw new JobHandlingException($"Failed ingesting entity ({job.EntityId}/{job.Culture}). Message: {message}");
            }
        }
    }
}
