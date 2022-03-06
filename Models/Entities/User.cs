using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            Document = new HashSet<Document>();
            Sign = new HashSet<Sign>();
            Template = new HashSet<Template>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(100)]
        public string Firstname { get; set; }

        [StringLength(100)]
        public string Lastname { get; set; }

        [StringLength(100)]
        public string Fathersname { get; set; }

        //[Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Document> Document { get; set; }

        //[Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Sign> Sign { get; set; }

        //[Newtonsoft.Json.JsonIgnore]   
        public virtual ICollection<Template> Template { get; set; }


        public string GetFIO()
        {
            if (Fathersname != null) return $"{Lastname} {Firstname[0]}. {Firstname[0]}.";
            else return $"{Lastname} {Firstname[0]}.";
        }
    }
}
