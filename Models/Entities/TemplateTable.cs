using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents.Models.Entities
{
    public class TemplateTable : TemplateItem
    {
        public TemplateTable()
        {
            TemplateFields = new HashSet<TemplateField>();
        }

        [DefaultValue(1)]
        public int Rows { get; set; }
        public override bool IsTable => true;

        public virtual ICollection<TemplateField> TemplateFields { get; set; }


        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
