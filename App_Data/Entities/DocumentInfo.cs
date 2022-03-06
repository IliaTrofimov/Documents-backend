using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models.Entities
{
    public class DocumentInfo
    {
        [ForeignKey("DocumentData")]
        public int Id { get; set; }
        public string Name { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public DateTime? ExpireDate { get; set; } = null;
        public int Type { get; set; }


        public int TemplateId { get; set; }
        public virtual Template Template { get; set; }
        public virtual DocumentData DocumentData { get; private set; }
        public virtual ICollection<Sign> Signs { get; private set; }
    }
}