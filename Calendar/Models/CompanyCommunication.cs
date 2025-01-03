
using System;
using System.ComponentModel.DataAnnotations;

namespace Calendar.Models
{
    public class CompanyCommunication
    {
        [Key]
        public Guid CompanyCommunicationId { get; set; }  

        [Required]
        public Guid CompanyId { get; set; }

        public CompanyManagement Company { get; set; }

       
        [Required]
        public List<Guid> CommunicationIds { get; set; } = new List<Guid>();
        public DateTime ScheduledDate { get; set; }
        public bool Status { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Description { get; set; }

    }
}
