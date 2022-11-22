using System;
using System.Net;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Exceptions;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Umbraco.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers.Media
{
    public class EnterspeedMediaTrashedJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;
        private readonly IMediaService _mediaService;

        public EnterspeedMediaTrashedJobHandler(
            IEnterspeedIngestService enterspeedIngestService,
            IEnterspeedConnectionProvider enterspeedConnectionProvider,
            IMediaService mediaService)
        {
            _enterspeedIngestService = enterspeedIngestService;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
            _mediaService = mediaService;
        }

        public bool CanHandle(EnterspeedJob job)
        {
            return
               _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish) != null
               && job.EntityType == EnterspeedJobEntityType.Media
               && job.JobType == EnterspeedJobType.Delete
               && job.ContentState == EnterspeedContentState.Publish;
        }

        public void Handle(EnterspeedJob job)
        {
            var parsed = int.TryParse(job.EntityId, out var parsedId);
            var media = parsed ? _mediaService.GetById(parsedId) : null;

            var deleteResponse = _enterspeedIngestService.Delete(media?.Id.ToString(), _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish));
            if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
            {
                throw new JobHandlingException($"Failed deleting entity ({job.EntityId}/{job.Culture}). Message: {deleteResponse.Message}");
            }
        }
    }
}
