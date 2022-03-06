using System.Data.Entity;

namespace Documents_backend.Models
{
    public partial class DataContext : DbContext
    {
        public DataContext()
            : base("name=DataContext")
        { }

        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentDataItem> DocumentDataItem { get; set; }
        public virtual DbSet<DocumentTableCell> DocumentTableCell { get; set; }
        public virtual DbSet<Sign> Sign { get; set; }
        public virtual DbSet<Template> Template { get; set; }
        public virtual DbSet<TemplateField> TemplateField { get; set; }
        public virtual DbSet<TemplateType> TemplateType { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasMany(e => e.DocumentDataItem)
                .WithRequired(e => e.Document)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Document>()
                .HasMany(e => e.DocumentTableCell)
                .WithRequired(e => e.Document)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Document>()
                .HasMany(e => e.Sign)
                .WithRequired(e => e.Document)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Template>()
                .HasMany(e => e.Document)
                .WithRequired(e => e.Template)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Template>()
                .HasMany(e => e.TemplateField)
                .WithRequired(e => e.Template)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TemplateType>()
                .HasMany(e => e.Template)
                .WithRequired(e => e.TemplateType)
                .HasForeignKey(e => e.TypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Document)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.AuthorId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Sign)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Template)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.AuthorId);
        }
    }
}
