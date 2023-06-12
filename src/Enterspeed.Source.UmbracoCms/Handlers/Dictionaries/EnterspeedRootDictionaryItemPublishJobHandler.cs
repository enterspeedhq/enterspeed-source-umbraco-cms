using System;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Exceptions;
using Enterspeed.Source.UmbracoCms.Models;
using Enterspeed.Source.UmbracoCms.Providers;
using Enterspeed.Source.UmbracoCms.Services;

namespace Enterspeed.Source.UmbracoCms.Handlers.Dictionaries
{
 public class EnterspeedRootDictionaryItemPublishJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;

        public EnterspeedRootDictionaryItemPublishJobHandler(
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
            return UmbracoDictionariesRootEntity.EntityId.Equals(job.EntityId, StringComparison.InvariantCultureIgnoreCase)
                &&  _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish) != null
                && job.EntityType == EnterspeedJobEntityType.Dictionary
                && job.ContentState == EnterspeedContentState.Publish
                && job.JobType == EnterspeedJobType.Publish;
        }

        public void Handle(EnterspeedJob job)
        {
            var umbracoData = CreateUmbracoDictionaryEntity(job);
            Ingest(umbracoData, job);
        }

        internal UmbracoDictionariesRootEntity CreateUmbracoDictionaryEntity(EnterspeedJob job)
        {
            try
            {
                return new UmbracoDictionariesRootEntity(
                    _entityIdentityService, job.Culture);
            }
            catch (Exception e)
            {
                throw new JobHandlingException(
                    $"Failed creating entity ({job.EntityId}/{job.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}");
            }
        }

        internal void Ingest(IEnterspeedEntity umbracoData, EnterspeedJob job)
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