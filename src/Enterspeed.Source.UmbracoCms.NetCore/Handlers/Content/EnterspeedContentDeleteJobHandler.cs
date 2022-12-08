using System.Net;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.NetCore.Data.Models;
using Enterspeed.Source.UmbracoCms.NetCore.Exceptions;
using Enterspeed.Source.UmbracoCms.NetCore.Models;
using Enterspeed.Source.UmbracoCms.NetCore.Providers;
using Enterspeed.Source.UmbracoCms.NetCore.Services;

namespace Enterspeed.Source.UmbracoCms.NetCore.Handlers.Content
{
    public class EnterspeedContentDeleteJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;

        public EnterspeedContentDeleteJobHandler(
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IEnterspeedConnectionProvider enterspeedConnectionProvider)
        {
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
        }

        public bool CanHandle(EnterspeedJob job)
        {
            return
                _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish) != null
                && job.ContentState == EnterspeedContentState.Publish
                && job.EntityType == EnterspeedJobEntityType.Content
                && job.JobType == EnterspeedJobType.Delete;
        }

        public void Handle(EnterspeedJob job)
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
