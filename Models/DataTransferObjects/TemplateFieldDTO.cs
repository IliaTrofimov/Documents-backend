namespace Documents_backend.Models
{
    public partial class TemplateFieldDTO
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public int TemplateId { get; set; }

        public string Restriction { get; set; }

        public int RestrictionType { get; set; }

        public bool Required { get; set; }

        public int DataType { get; set; }

        public int? Row { get; set; }

        public int? Col { get; set; }
    }
}
