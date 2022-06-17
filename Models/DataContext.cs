using Documents_Entities.Entities;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;


namespace Documents_backend
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<DocumentDataItem> DocumentDataItems { get; set; }
        public virtual DbSet<Sign> Signs { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateField> TemplateFields { get; set; }
        public virtual DbSet<TemplateTable> TemplateTables { get; set; }
        public virtual DbSet<TemplateType> TemplateTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Position> Positions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var convention = new AttributeToColumnAnnotationConvention<DefaultValueAttribute, string>(
                "SqlDefaultValue", (p, attributes) => attributes.SingleOrDefault().Value.ToString()
            );
            modelBuilder.Conventions.Add(convention);

            modelBuilder.Entity<TemplateItem>()
                .Map<TemplateField>(m => m.Requires("IsTable").HasValue(false))
                .Map<TemplateTable>(m => m.Requires("IsTable").HasValue(true));

            modelBuilder.Entity<TemplateItem>()
                .HasRequired(t => t.Template)
                .WithMany()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DocumentDataItem>()
                .HasRequired(i => i.Document)
                .WithMany()
                .WillCascadeOnDelete(true);
        }
    }
}
