using System.ComponentModel.DataAnnotations;

namespace Documents_backend.Models.Entities
{
    public class DocumentData
    {
        public int Id { get; set; }
        public string Fields { get; set; } = "[]";
        public string Tables { get; set; } = "[]";
    }
}