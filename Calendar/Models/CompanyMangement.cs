
using System.ComponentModel.DataAnnotations;

namespace Calendar.Models
{
    public class CompanyManagement
    {
        [Key]
        public Guid CompanyId { get; set; } 
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public string LinkedInProfile { get; set; }
        public string Emails { get; set; }
        public string PhoneNumbers { get; set; }
        public string Comments { get; set; }
        public string CommunicationPeriodicity { get; set; }  

        public DateTime CreatedDate { get; set; }
       
    }
}
