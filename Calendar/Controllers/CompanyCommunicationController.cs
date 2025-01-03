
using Calendar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Calendar.Helpers;
using Calendar.DTOs;



namespace Calendar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyCommunicationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly GlobalData _globalData = new GlobalData();

        public CompanyCommunicationController(ApplicationDbContext context)
        {
            _context = context;
        }


       
        [HttpGet]
        [Route("GetAllCompanyCommunications")]
        public async Task<IActionResult> GetAllCompanyCommunications()
        {
            try
            {
                var companyCommunications = await _context.CompanyCommunications
                    .Include(cc => cc.Company)
                    .ToListAsync();

                var communicationManagements = await _context.CommunicationManagements
                    .ToListAsync();

                var result = companyCommunications
                    .GroupBy(cc => new { cc.CompanyId, cc.Company.CompanyName })
                    .Select(group => new
                    {
                        CompanyId = group.Key.CompanyId,
                        CompanyName = group.Key.CompanyName,
                        Communications = group
                            .SelectMany(cc => cc.CommunicationIds
                                .Select(communicationId => communicationManagements
                                    .Where(c => c.CommunicationId == communicationId)
                                    .Select(c => new
                                    {
                                        CompanyCommunicationId = cc.CompanyCommunicationId,
                                        CommunicationId=c.CommunicationId,
                                        CommunicationName = c.CommunicationName,
                                        ScheduledDate = cc.ScheduledDate,
                                        UpdatedDate = cc.UpdatedDate,
                                        Status = cc.Status,
                                        Description=cc.Description,
                                    })
                                    .FirstOrDefault()))
                            .OrderBy(c => c.ScheduledDate)
                            .ToList()
                    })
                    .ToList();

                return Ok(new { StatusCode = HttpStatusCode.OK, Data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.CompanyCommunications.ToListAsync();
                return Ok(new { StatusCode = HttpStatusCode.OK, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }
        [HttpPut]
        [Route("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] object request)
        {
            try
            {
                var payload = JsonConvert.DeserializeObject<UpdateStatusRequest>(request.ToString());
                var selectedCompanies = payload.SelectedCompanies;  
                var selectedCommunication = payload.SelectedCommunication;  
                var selectedDate = payload.SelectedDate;  
                var notes = payload.Notes;  

                
                if (selectedCommunication == Guid.Empty)
                {
                    return BadRequest("Communication ID is missing or invalid.");
                }
                foreach (var companyId in selectedCompanies)
                {
                    Console.WriteLine($"Processing companyId: {companyId}, communicationId: {selectedCommunication}");

                    var companyCommunication = await _context.CompanyCommunications
     .Where(c => c.CompanyId == companyId && c.CommunicationIds.Contains(selectedCommunication))
     .FirstOrDefaultAsync();

                    if (companyCommunication != null)
                    {
                        Console.WriteLine($"Found company communication: {companyCommunication}");

                        companyCommunication.Status = true; 
                        companyCommunication.UpdatedDate = _globalData.GetLocalTime(DateTime.UtcNow);
                        companyCommunication.Description = notes; 
                        _context.CompanyCommunications.Update(companyCommunication);
                    }
                    else
                    {
                        Console.WriteLine("No matching CompanyCommunication found.");
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { statusCode = 200, message = "Status updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { statusCode = 500, message = ex.Message });
            }
        }
        [HttpGet("GetCompanyCommunications")]
        public async Task<IActionResult> GetCompanyCommunications()
        {
            var communications = await (from cc in _context.CompanyCommunications
                                        join c in _context.CompanyManagements on cc.CompanyId equals c.CompanyId
                                        join comm in _context.CommunicationManagements on cc.CommunicationIds.First() equals comm.CommunicationId // Adjust for multiple IDs if needed
                                        select new CompanyCommunicationResponseDto
                                        {
                                            CompanyName = c.CompanyName,
                                            CommunicationType = comm.CommunicationName, 
                                            ScheduledDate = cc.ScheduledDate
                                        }).ToListAsync();

            return Ok(communications);
        }
        [HttpGet("status")]
        public async Task<IActionResult> GetCompanyCommunicationsStatus()
        {
            var today = DateTime.UtcNow.Date;

            var communications = await _context.CompanyCommunications
                .Include(cc => cc.Company)
                .OrderBy(cc => cc.ScheduledDate)
                .ToListAsync();

            var result = new List<object>();

            var groupedCommunications = communications.GroupBy(cc => cc.CompanyId);

            foreach (var group in groupedCommunications)
            {
                var companyName = group.First().Company?.CompanyName;
                var overdueCommunications = new List<object>();
                var dueTodayCommunications = new List<object>();
                var orderedCommunications = group.OrderBy(c => c.ScheduledDate).ToList();
                foreach (var communication in orderedCommunications)
                {
                    if (!communication.Status)
                    {
                        if (communication.ScheduledDate.Date < today)
                        {
                            overdueCommunications.Add(new
                            {
                                communication.Description,
                                communication.ScheduledDate,
                                DaysOverdue = (today - communication.ScheduledDate.Date).Days
                            });
                        }
                        else if (communication.ScheduledDate.Date == today)
                        {
                            dueTodayCommunications.Add(new
                            {
                                communication.Description,
                                communication.ScheduledDate
                            });
                        }
                    }
                }

                if (overdueCommunications.Any() || dueTodayCommunications.Any())
                {
                    result.Add(new
                    {
                        CompanyName = companyName,
                        Overdue = overdueCommunications,
                        DueToday = dueTodayCommunications
                    });
                }
            }

            return Ok(result);
        }
       
        [HttpPut]
        [Route("UpdateScheduledDate")]
        public async Task<IActionResult> UpdateScheduledDate([FromBody] UpdateScheduledDateRequest request)
        {
            try
            {
                if (request.CompanyCommunicationId == Guid.Empty || request.ScheduledDate == default)
                {
                    return BadRequest(new { statusCode = 400, message = "Invalid CompanyCommunicationId or ScheduledDate." });
                }

  
                var companyCommunication = await _context.CompanyCommunications
                    .FirstOrDefaultAsync(cc => cc.CompanyCommunicationId == request.CompanyCommunicationId);

                if (companyCommunication == null)
                {
                    return NotFound(new { statusCode = 404, message = "CompanyCommunication not found." });
                }

                companyCommunication.ScheduledDate = request.ScheduledDate;
               

                _context.CompanyCommunications.Update(companyCommunication);
                await _context.SaveChangesAsync();

                return Ok(new { statusCode = 200, message = "Scheduled date updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { statusCode = 500, message = ex.Message });
            }
        }
        [HttpGet("GetNotificationCounts")]
        public async Task<IActionResult> GetNotificationCounts()
        {
            var today = DateTime.UtcNow.Date;

            var communications = await _context.CompanyCommunications
                .Where(cc => !cc.Status) 
                .ToListAsync();

            var overdueCount = communications.Count(cc => cc.ScheduledDate.Date < today);
            var dueTodayCount = communications.Count(cc => cc.ScheduledDate.Date == today);

            return Ok(new
            {
                OverdueCount = overdueCount,
                DueTodayCount = dueTodayCount,
                TotalCount = overdueCount + dueTodayCount
            });
        }





    }
}
