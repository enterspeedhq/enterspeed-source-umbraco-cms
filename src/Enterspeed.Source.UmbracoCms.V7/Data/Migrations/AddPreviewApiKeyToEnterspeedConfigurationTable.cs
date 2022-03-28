using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Enterspeed.Source.UmbracoCms.V7.Data.Migrations
{
    [Migration("2.0.0", 1, "EnterspeedConfiguration")]
    public class AddPreviewApiKeyToEnterspeedConfigurationTable : MigrationBase
    {
        private readonly string ConfigurationTableName = "EnterspeedConfiguration";
        public AddPreviewApiKeyToEnterspeedConfigurationTable(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
        }

        public override void Up()
        {
            Alter.Table(ConfigurationTableName)
                .AddColumn("PreviewApiKey").AsString(255).Nullable();
        }

        public override void Down()
        {
            Delete.Column("PreviewApiKey").FromTable(ConfigurationTableName);
        }
    }
}
