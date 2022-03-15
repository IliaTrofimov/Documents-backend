using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;

namespace Documents_backend.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Models.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SqlClient", new DefaultValueSqlServerMigrationSqlGenerator());
        }
    }

    internal class DefaultValueSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        private int dropConstraintCount;

        protected override void Generate(AddColumnOperation addColumnOperation)
        {
            SetAnnotatedColumn(addColumnOperation.Column, addColumnOperation.Table);
            base.Generate(addColumnOperation);
        }

        protected override void Generate(AlterColumnOperation alterColumnOperation)
        {
            SetAnnotatedColumn(alterColumnOperation.Column, alterColumnOperation.Table);
            base.Generate(alterColumnOperation);
        }

        protected override void Generate(CreateTableOperation createTableOperation)
        {
            SetAnnotatedColumns(createTableOperation.Columns, createTableOperation.Name);
            base.Generate(createTableOperation);
        }

        protected override void Generate(AlterTableOperation alterTableOperation)
        {
            SetAnnotatedColumns(alterTableOperation.Columns, alterTableOperation.Name);
            base.Generate(alterTableOperation);
        }

        private void SetAnnotatedColumn(ColumnModel column, string tableName)
        {
            if (column.Annotations.TryGetValue("SqlDefaultValue", out var values))
            {
                if (values.NewValue == null)
                {
                    column.DefaultValueSql = null;
                    using (var writer = Writer())
                    {
                        writer.WriteLine(GetSqlDropConstraintQuery(tableName, column.Name));
                        Statement(writer);
                    }
                }
                else
                {
                    column.DefaultValueSql = (string)values.NewValue;
                }
            }
        }

        private void SetAnnotatedColumns(IEnumerable<ColumnModel> columns, string tableName)
        {
            foreach (var column in columns)
            {
                SetAnnotatedColumn(column, tableName);
            }
        }

        private string GetSqlDropConstraintQuery(string tableName, string columnName)
        {
            var tableNameSplitByDot = tableName.Split('.');
            var tableSchema = tableNameSplitByDot[0];
            var tablePureName = tableNameSplitByDot[1];

            var str = $@"DECLARE @var{dropConstraintCount} nvarchar(128)
SELECT @var{dropConstraintCount} = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'{tableSchema}.[{tablePureName}]')
AND col_name(parent_object_id, parent_column_id) = '{columnName}';
IF @var{dropConstraintCount} IS NOT NULL
EXECUTE('ALTER TABLE {tableSchema}.[{tablePureName}] DROP CONSTRAINT [' + @var{dropConstraintCount} + ']')";

            dropConstraintCount++;
            return str;
        }
    }
}
