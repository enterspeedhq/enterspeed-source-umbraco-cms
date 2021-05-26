using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Enterspeed.Source.UmbracoCms.V7.Data.Schemas
{
    [TableName("EnterspeedConfiguration")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class EnterspeedConfigurationSchema
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public int ConnectionTimeout { get; set; }
        public string IngestVersion { get; set; }
        public string MediaDomain { get; set; }
    }
}
