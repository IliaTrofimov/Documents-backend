using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Documents_backend.Models.Entities
{
    public class DocumentDataItem
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public int FieldId { get; set; }
        public virtual InputField InputField { get; set; }
    }
}