using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Enterspeed.Source.UmbracoCms.V7.Data.Migrations
{
    [Migration("3.0.0", 1, "EnterspeedJobs")]
    public class AddContentStateToEnterspeedJobsTable : MigrationBase
    {
        private readonly string JobsTableName = "EnterspeedJobs";
        public AddContentStateToEnterspeedJobsTable(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
        }

        public override void Up()
        {
            Alter.Table(JobsTableName)
                .AddColumn("ContentState").AsInt32().NotNullable()
                .WithDefaultValue((int)EnterspeedContentState.Publish);
        }

        public override void Down()
        {
            Delete.Column("ContentState").FromTable(JobsTableName);
        }
    }
}
