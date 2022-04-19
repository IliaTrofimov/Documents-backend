using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;
using Documents_backend.Models;

namespace Documents_backend.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SqlClient", new DefaultValueSqlServerMigrationSqlGenerator());
        }

        protected override void Seed(DataContext context)
        {
            return;
            context.TemplateFields.RemoveRange(context.TemplateFields);
            context.TemplateTables.RemoveRange(context.TemplateTables);
            context.Templates.RemoveRange(context.Templates);
            context.Users.RemoveRange(context.Users);
            context.TemplateTypes.RemoveRange(context.TemplateTypes);

            string[] names = new string[] { "Илья", "Антон", "Пётр", "Иван", "Алексей" };
            string[] lastnames = new string[] { "Иванов", "Гаврилов", "Сидоров", "Соколов", "Трофимов" };

            for (int i = 1; i <= 5; i++)
            {
                context.TemplateTypes.Add(new TemplateType() { Name = $"Тип {i}" });
                context.Users.Add(new User() { Firstname = names[i - 1], Lastname = lastnames[i - 1] });
            }
               

            for (int i = 1; i <= 10; i++)
            {
                Template t = new Template() { 
                    Name = $"Шаблон {i}", 
                };

                for (int j = 1; j <= 6 && j < i; j++)
                {
                    TemplateField f = new TemplateField() {
                        Name = $"Поле {i}-{j}", Template = t, Order = j - 1 
                    };
                    t.TemplateItems.Add(f);
                    context.TemplateFields.Add(f);
                }

                if (i > 6)
                {
                    TemplateTable tt = new TemplateTable() {
                        Name = "Таблица 1", Template = t, Order = 6, Rows = 5
                    };
                    context.TemplateTables.Add(tt);

                    for (int j = 1; j <= 3; j++)
                    {
                        TemplateField c = new TemplateField() {
                            Name = $"Колонка {i}-1-{j}", TemplateTable = tt , Order = j - 1, Template = t  
                        };
                        tt.TemplateFields.Add(c);
                        t.TemplateItems.Add(c);
                        context.TemplateFields.Add(c);
                    }
                }
            }

            context.SaveChanges();
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
