namespace Calendar.DTOs
{
    public class UpdateScheduledDateRequest
    {
        public Guid CompanyCommunicationId { get; set; }
        public DateTime ScheduledDate { get; set; }
    }

}