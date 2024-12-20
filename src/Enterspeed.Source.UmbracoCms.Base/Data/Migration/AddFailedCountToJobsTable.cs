﻿using Enterspeed.Source.UmbracoCms.Base.Data.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Migration
{
    public class AddFailedCountToJobsTable : MigrationBase
    {
        public AddFailedCountToJobsTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            var jobsTable = Database.PocoDataFactory.ForType(typeof(EnterspeedJobSchema));
            var jobsTableName = jobsTable.TableInfo.TableName;

            const string entityTypeColumnName = "FailedCount";

            // If column doesn't exist, create it
            if (!ColumnExists(jobsTableName, entityTypeColumnName))
            {
                Create
                    .Column(entityTypeColumnName)
                    .OnTable(jobsTableName)
                    .AsInt32().NotNullable().WithDefaultValue(0)
                    .Do();
            }
        }
    }
}
