namespace Documents_backend.Models.Entities
{
    public class InputField : TemplateRow
    {
        public int RestrictionType { get; set; }
        public int DataType { get; set; }
        public bool Required { get; set; }
        public string Restriction { get; set; } = null;


        public int? TableId { get; set; } = null;
        public virtual InputTable InputTable { get; set; }
    }
}