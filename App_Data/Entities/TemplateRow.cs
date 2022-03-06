namespace Documents_backend.Models.Entities
{
    public abstract class TemplateRow
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }


        public virtual int TemplateId { get; set; }
        public virtual Template Template { get; set; }
    }
}
