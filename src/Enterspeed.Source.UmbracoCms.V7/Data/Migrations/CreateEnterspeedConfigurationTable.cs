using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Enterspeed.Source.UmbracoCms.V7.Data.Migrations
{
    [Migration("1.0.0", 1, "EnterspeedConfiguration")]
    public class CreateEnterspeedConfigurationTable : MigrationBase
    {
        private readonly string ConfigurationTableName = "EnterspeedConfiguration";
        public CreateEnterspeedConfigurationTable(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
        }

        public override void Up()
        {
            Create.Table(ConfigurationTableName)
                .WithColumn("Id").AsInt32().PrimaryKey("PK_EnterspeedConfiguration").Identity()
                .WithColumn("ApiKey").AsString(255).NotNullable()
                .WithColumn("BaseUrl").AsString(255).NotNullable()
                .WithColumn("ConnectionTimeout").AsInt32().NotNullable()
                .WithColumn("IngestVersion").AsString(255).NotNullable()
                .WithColumn("MediaDomain").AsString(255).NotNullable();
        }

        public override void Down()
        {
            Delete.Table(ConfigurationTableName);
        }
    }
}
