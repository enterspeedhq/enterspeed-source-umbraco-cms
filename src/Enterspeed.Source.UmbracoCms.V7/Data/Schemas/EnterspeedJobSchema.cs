using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Enterspeed.Source.UmbracoCms.V7.Data.Schemas
{
    [TableName("EnterspeedJobs")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class EnterspeedJobSchema
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("EntityId")]
        public string EntityId { get; set; }

        [Column("Culture")]
        public string Culture { get; set; }

        [Column("JobType")]
        public int JobType { get; set; }

        [Column("JobState")]
        public int JobState { get; set; }

        [Column("Exception")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Exception { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        [Column("EntityType")]
        public int EntityType { get; set; }

        [Column("ContentState")]
        public int ContentState { get; set; }
    }
}
