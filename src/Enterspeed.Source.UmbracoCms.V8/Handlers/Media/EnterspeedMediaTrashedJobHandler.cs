using System.Net;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Exceptions;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Umbraco.Core.Logging;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers.Media
{
    public class EnterspeedMediaTrashedJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;
        private readonly IProfilingLogger _logger;

        public EnterspeedMediaTrashedJobHandler(
            IEnterspeedIngestService enterspeedIngestService,
            IEnterspeedConnectionProvider enterspeedConnectionProvider,
            IProfilingLogger logger)
        {
            _enterspeedIngestService = enterspeedIngestService;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
            _logger = logger;
        }

        public virtual bool CanHandle(EnterspeedJob job)
        {
            return
               _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish) != null
               && job.EntityType == EnterspeedJobEntityType.Media
               && job.JobType == EnterspeedJobType.Delete
               && job.ContentState == EnterspeedContentState.Publish;
        }

        public virtual void Handle(EnterspeedJob job)
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
                _logger.Warn<EnterspeedMediaTrashedJobHandler>("Job.EntityId is not a valid ID");
            }
        }
    }
}
