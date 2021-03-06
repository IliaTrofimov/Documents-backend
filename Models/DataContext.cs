using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

using Documents.Models.Entities;


namespace Documents.Models
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

        public virtual DbSet<HtmlTemplate> HtmlTemplates { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var convention = new AttributeToColumnAnnotationConvention<DefaultValueAttribute, string>(
                "SqlDefaultValue", (p, attributes) => attributes.SingleOrDefault().Value.ToString()
            );
            modelBuilder.Conventions.Add(convention);

            modelBuilder.Entity<TemplateItem>()
                .Map<TemplateField>(m => m.Requires("IsTable").HasValue(false))
                .Map<TemplateTable>(m => m.Requires("IsTable").HasValue(true));

            modelBuilder.Entity<Sign>()
                .HasRequired<User>(s => s.User)
                .WithMany(u => u.Signs)
                .HasForeignKey(s => s.UserCWID);

            Configuration.LazyLoadingEnabled = true;
        }
    }
}
