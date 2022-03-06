using Documents_backend.Models.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Documents_backend.Models
{
    public class DataContext : DbContext
    {
        public DbSet<DocumentData> Data { get; set; }
        public DbSet<DocumentInfo> Info { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateType> TemplateTypes { get; set; }
        public DbSet<InputField> InputFields { get; set; }
        public DbSet<InputTable> InputTables { get; set; }
        public DbSet<DocumentDataItem> DataItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Sign> Signs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();


            base.OnModelCreating(modelBuilder);
        }
    }
}