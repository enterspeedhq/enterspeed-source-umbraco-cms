using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Schemas;
using Umbraco.Cms.Core.Mapping;

namespace Enterspeed.Source.UmbracoCms.V9.Data.MappingDefinitions
{
    public class EnterspeedJobMappingDefinition : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<EnterspeedJobSchema, EnterspeedJob>((source, context) => new EnterspeedJob(), MapTo);
            mapper.Define<EnterspeedJob, EnterspeedJobSchema>((source, context) => new EnterspeedJobSchema(), MapFrom);
        }
        
        private void MapTo(EnterspeedJobSchema source, EnterspeedJob target, MapperContext context)
        {
            target.Id = source.Id;
            target.EntityId = source.EntityId;
            target.CreatedAt = source.CreatedAt;
            target.UpdatedAt = source.UpdatedAt;
            target.JobType = (EnterspeedJobType)source.JobType;
            target.State = (EnterspeedJobState)source.JobState;
            target.Exception = source.Exception;
            target.Culture = source.Culture;
            target.EntityType = (EnterspeedJobEntityType)source.EntityType;
        }

        private void MapFrom(EnterspeedJob source, EnterspeedJobSchema target, MapperContext context)
        {
            target.Id = source.Id;
            target.EntityId = source.EntityId;
            target.CreatedAt = source.CreatedAt;
            target.UpdatedAt = source.UpdatedAt;
            target.JobType = source.JobType.GetHashCode();
            target.JobState = source.State.GetHashCode();
            target.Exception = source.Exception;
            target.Culture = source.Culture;
            target.EntityType = source.EntityType.GetHashCode();
        }
    }
}