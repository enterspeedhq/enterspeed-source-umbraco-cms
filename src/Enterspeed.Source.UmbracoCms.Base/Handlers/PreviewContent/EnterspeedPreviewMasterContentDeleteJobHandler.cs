using System.Net;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Exceptions;
using Enterspeed.Source.UmbracoCms.Models;
using Enterspeed.Source.UmbracoCms.Providers;
using Enterspeed.Source.UmbracoCms.Services;

namespace Enterspeed.Source.UmbracoCms.Handlers.PreviewContent
{
    public class EnterspeedPreviewMasterContentDeleteJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;

        public EnterspeedPreviewMasterContentDeleteJobHandler(
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
                _enterspeedConnectionProvider.GetConnection(ConnectionType.Preview) != null
                && job.ContentState == EnterspeedContentState.Preview
                && job.EntityType == EnterspeedJobEntityType.MasterContent
                && job.JobType == EnterspeedJobType.Delete;
        }

        public virtual void Handle(EnterspeedJob job)
        {
            var id = _entityIdentityService.GetId(job.EntityId);
            var deleteResponse = _enterspeedIngestService.Delete(id, _enterspeedConnectionProvider.GetConnection(ConnectionType.Preview));
            if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
            {
                throw new JobHandlingException($"Failed deleting entity ({job.EntityId}). Message: {deleteResponse.Message}");
            }
        }
    }
}