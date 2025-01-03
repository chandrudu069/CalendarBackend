
using System.ComponentModel.DataAnnotations;

namespace Calendar.Models
{
    public class CommunicationManagement
    {
        [Key]
        public Guid CommunicationId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string CommunicationName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sequence must be a positive number.")]
        public int Sequence { get; set; }

        public bool Mandatory { get; set; }
    }
}
