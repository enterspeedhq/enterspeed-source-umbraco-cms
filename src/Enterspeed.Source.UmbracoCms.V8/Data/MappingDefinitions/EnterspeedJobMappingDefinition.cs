using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Schemas;
using Umbraco.Core.Mapping;

namespace Enterspeed.Source.UmbracoCms.V8.Data.MappingDefinitions
{
    public class EnterspeedJobMappingDefinition : IMapDefinition
    {
        public void DefineMaps(UmbracoMapper mapper)
        {
            mapper.Define<EnterspeedJobSchema, EnterspeedJob>((source, context) => new EnterspeedJob(), MapTo);
            mapper.Define<EnterspeedJob, EnterspeedJobSchema>((source, context) => new EnterspeedJobSchema(), MapFrom);
        }

        private void MapTo(EnterspeedJobSchema source, EnterspeedJob target, MapperContext context)
        {
            target.Id = source.Id;
            target.ContentId = source.ContentId;
            target.CreatedAt = source.CreatedAt;
            target.UpdatedAt = source.UpdatedAt;
            target.JobType = (EnterspeedJobType)source.JobType;
            target.State = (EnterspeedJobState)source.JobState;
            target.Exception = source.Exception;
            target.Culture = source.Culture;
        }

        private void MapFrom(EnterspeedJob source, EnterspeedJobSchema target, MapperContext context)
        {
            target.Id = source.Id;
            target.ContentId = source.ContentId;
            target.CreatedAt = source.CreatedAt;
            target.UpdatedAt = source.UpdatedAt;
            target.JobType = source.JobType.GetHashCode();
            target.JobState = source.State.GetHashCode();
            target.Exception = source.Exception;
            target.Culture = source.Culture;
        }
    }
}