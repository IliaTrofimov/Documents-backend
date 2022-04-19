using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    public partial class TemplateTable : TemplateItem
    {
        public TemplateTable()
        {
            TemplateFields = new HashSet<TemplateField>();
        }

        [DefaultValue(1)]
        public int Rows { get; set; }

        public virtual ICollection<TemplateField> TemplateFields { get; set; }
    }
}
