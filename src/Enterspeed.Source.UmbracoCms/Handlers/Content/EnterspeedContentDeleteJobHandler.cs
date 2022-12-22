using System.Net;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Handlers;
using Enterspeed.Source.UmbracoCms.Models;
using Enterspeed.Source.UmbracoCms.Exceptions;
using Enterspeed.Source.UmbracoCms.Providers;
using Enterspeed.Source.UmbracoCms.Services;

namespace Enterspeed.Source.UmbracoCms.Handlers.Content
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
