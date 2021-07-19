using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Enterspeed.Source.UmbracoCms.V7.Data.Migrations
{
    [Migration("1.0.0", 1, "EnterspeedJobs")]
    public class CreateEnterspeedJobsTable : MigrationBase
    {
        private readonly string JobsTableName = "EnterspeedJobs";
        public CreateEnterspeedJobsTable(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
        }

        public override void Up()
        {
            Create.Table(JobsTableName)
                .WithColumn("Id").AsInt32().PrimaryKey("PK_EnterspeedJobs").Identity()
                .WithColumn("ContentId").AsInt32().NotNullable()
                .WithColumn("Culture").AsString(10).NotNullable()
                .WithColumn("JobType").AsInt32().NotNullable()
                .WithColumn("JobState").AsInt32().NotNullable()
                .WithColumn("Exception").AsString().Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table(JobsTableName);
        }
    }
}
