using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models.Entities
{
    public class Sign
    {
        public int Id { get; set; }
        public bool Signed { get; set; } = false;
        
        public User Signer { get; set; }
        public int SignerrId { get; set; }
        public DocumentInfo DocumentInfo { get; set; }
        public int DocumentInfoId { get; set; }
    }
}