using System.Net;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Exceptions;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers
{
    public class EnterspeedDictionaryItemDeleteJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;

        public EnterspeedDictionaryItemDeleteJobHandler(
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IEnterspeedConnectionProvider enterspeedConnectionProvider)
        {
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
        }

        public virtual bool CanHandle(EnterspeedJob job)
        {
            return
               _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish) != null
               && job.EntityType == EnterspeedJobEntityType.Dictionary
               && job.JobType == EnterspeedJobType.Delete
               && job.ContentState == EnterspeedContentState.Publish;
        }

        public virtual void Handle(EnterspeedJob job)
        {
            var id = _entityIdentityService.GetId(job.EntityId, job.Culture);
            var deleteResponse = _enterspeedIngestService.Delete(id, _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish));
            if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
            {
                throw new JobHandlingException($"Failed deleting entity ({job.EntityId}/{job.Culture}). Message: {deleteResponse.Message}");
            }
        }
    }
}
