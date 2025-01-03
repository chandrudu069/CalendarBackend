
using System;
using System.Collections.Generic;

namespace Calendar.Models
{
    public class UpdateStatusRequest
    {
        public List<Guid> SelectedCompanies { get; set; }
        public Guid SelectedCommunication { get; set; }
        public List<DateTime> SelectedDate { get; set; }
        public string Notes { get; set; }
    }
}
