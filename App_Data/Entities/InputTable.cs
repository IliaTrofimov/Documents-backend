using System.Collections.Generic;


namespace Documents_backend.Models.Entities
{
    public class InputTable : TemplateRow
    {
        public int Rows { get; set; }
        public ICollection<InputField> Columns { get; set; }
    }
}