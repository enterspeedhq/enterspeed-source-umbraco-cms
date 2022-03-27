using System;
using System.Net;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Exceptions;
using Enterspeed.Source.UmbracoCms.V8.Services;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
{
    public class EnterspeedDeletedContentJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;

        public EnterspeedDeletedContentJobHandler(
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService)
        {
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
        }

        public bool CanHandle(EnterspeedJob job)
        {
            return
                (job.EntityType == EnterspeedJobEntityType.Content)
                && job.JobType == EnterspeedJobType.Delete;
        }

        public void Handle(EnterspeedJob job)
        {
            var id = _entityIdentityService.GetId(job.EntityId, job.Culture);
            var deleteResponse = _enterspeedIngestService.Delete(id);
            if (!deleteResponse.Success && deleteResponse.Status != HttpStatusCode.NotFound)
            {
                throw new JobHandlingException(
                    $"Failed deleting entity ({job.EntityId}/{job.Culture}). Message: {deleteResponse.Message}");
            }
        }
    }
}