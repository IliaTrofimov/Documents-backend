using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents.Models.Entities
{
    public class TemplateField : TemplateItem
    {
        [DefaultValue("")]
        public string Restriction { get; set; } = string.Empty;

        [DefaultValue(0)]
        public int? RestrictionType { get; set; } = 0;

        [DefaultValue(0)]
        public bool? Required { get; set; } = false;

        [DefaultValue(0)]
        public int? DataType { get; set; } = 0;

        [JsonIgnore]
        public virtual TemplateTable TemplateTable { get; set; }
        public int? TemplateTableId { get; set; }

        [JsonIgnore]
        public virtual ICollection<DocumentDataItem> DocumentDataItems { get; set; }

        public override bool IsTable => false;


        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
