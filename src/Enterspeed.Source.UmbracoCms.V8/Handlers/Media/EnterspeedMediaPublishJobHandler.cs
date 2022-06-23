using System;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Exceptions;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers.Media
{
    public class EnterspeedMediaPublishJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IEnterspeedGuardService _enterspeedGuardService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;
        private readonly IMediaService _mediaService;

        public EnterspeedMediaPublishJobHandler(
            IEnterspeedPropertyService enterspeedPropertyService,
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IEnterspeedGuardService enterspeedGuardService,
            IEnterspeedConnectionProvider enterspeedConnectionProvider,
            IMediaService mediaService)
        {
            _enterspeedPropertyService = enterspeedPropertyService;
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _enterspeedGuardService = enterspeedGuardService;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
            _mediaService = mediaService;
        }

        public bool CanHandle(EnterspeedJob job)
        {
            return
                _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish) != null
                && job.EntityType == EnterspeedJobEntityType.Media
                && job.ContentState == EnterspeedContentState.Publish
                && job.JobType == EnterspeedJobType.Publish;
        }

        public void Handle(EnterspeedJob job)
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
            var isMediaId = Guid.TryParse(job.EntityId, out var mediaId);

            var media = isMediaId ? _mediaService.GetById(mediaId) : null;
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
                return new UmbracoMediaEntity(media, _enterspeedPropertyService, _entityIdentityService);
            }
            catch (Exception e)
            {
                throw new JobHandlingException(
                    $"Failed creating entity ({job.EntityId}). Message: {e.Message}. StackTrace: {e.StackTrace}");
            }
        }

        protected virtual void Ingest(IEnterspeedEntity umbracoData, EnterspeedJob job)
        {
            var ingestResponse = _enterspeedIngestService.Save(umbracoData, _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish));
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
