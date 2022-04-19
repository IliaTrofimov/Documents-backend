﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{

    public abstract partial class TemplateItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Order { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public int TemplateId { get; set; }
        [JsonIgnore]
        public virtual Template Template { get; set; }
    }
}