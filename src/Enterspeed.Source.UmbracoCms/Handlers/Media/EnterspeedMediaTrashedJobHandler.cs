﻿using System.Net;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Models;
using Enterspeed.Source.UmbracoCms.Exceptions;
using Enterspeed.Source.UmbracoCms.Providers;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Source.UmbracoCms.Handlers.Media
{
    public class EnterspeedMediaTrashedJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;
        private readonly ILogger<EnterspeedMediaTrashedJobHandler> _logger;

        public EnterspeedMediaTrashedJobHandler
        (
            IEnterspeedIngestService enterspeedIngestService,
            IEnterspeedConnectionProvider enterspeedConnectionProvider,
            ILogger<EnterspeedMediaTrashedJobHandler> logger)
        {
            _enterspeedIngestService = enterspeedIngestService;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
            _logger = logger;
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

            if (parsed)
            {
                var deleteResponse = _enterspeedIngestService.Delete(parsedId.ToString(), _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish));
                if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
                {
                    throw new JobHandlingException($"Failed deleting entity ({job.EntityId}/{job.Culture}). Message: {deleteResponse.Message}");
                }
            }
            else
            {
                _logger.LogWarning("Job.EntityId is not a valid ID");
            }
        }
    }
}
