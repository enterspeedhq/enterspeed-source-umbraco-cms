using System;
using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Enterspeed.Source.UmbracoCms.V8.Data.Schemas
{
    [TableName("umbracoKeyValue")]
    [PrimaryKey("key")]
    public class EnterspeedConfigurationSchema
    {
        [PrimaryKeyColumn]
        [Column("key")]
        public string Key { get; set; }

        [Column("value")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Value { get; set; }

        [Column("updated")]
        public DateTime Updated { get; set; }
    }
}
