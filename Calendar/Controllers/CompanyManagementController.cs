using Calendar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Calendar.Configuration;
using Calendar.Helpers;
using Microsoft.AspNetCore.Http;

namespace Calendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly GlobalData _globalData = new GlobalData();

        public CompanyManagementController(ApplicationDbContext context)
        {
            _context = context;
            _pagination = new Pagination();
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.CompanyManagements.ToListAsync();
                return Ok(new { StatusCode = HttpStatusCode.OK, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationFilter filter)
        {
            try
            {
                var data = await _context.CompanyManagements.ToListAsync();

                var pagedResponse = _pagination.GetPagination(data, filter);

                return Ok(new { StatusCode = HttpStatusCode.OK, Data = pagedResponse });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CompanyManagement model)
        {
            try
            {
                if (model == null)
                {
                    return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = "Check the Payload!" });
                }

                if (!ModelState.IsValid)
                {
                    return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = "Validation Error!" });
                }
                model.CompanyId = Guid.NewGuid();
                model.CreatedDate = _globalData.GetLocalTime(DateTime.UtcNow);
                var communications = await _context.CommunicationManagements
                    .OrderBy(c => c.Sequence)
                    .ToListAsync();

                if (!communications.Any())
                {
                    return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = "No communications found to link!" });
                }
                _context.CompanyManagements.Add(model);
                await _context.SaveChangesAsync();

                int communicationPeriodicityDays = int.Parse(model.CommunicationPeriodicity) * 7;
                DateTime baseScheduledDate = model.CreatedDate.AddDays(communicationPeriodicityDays); 
               
                foreach (var communication in communications.OrderBy(c => c.Sequence))
                {
                    var companyCommunication = new CompanyCommunication
                    {
                        CompanyCommunicationId = Guid.NewGuid(),
                        CompanyId = model.CompanyId,
                        CommunicationIds = new List<Guid> { communication.CommunicationId }, // Single CommunicationId per entry
                        ScheduledDate = baseScheduledDate.AddDays(communicationPeriodicityDays * communications.IndexOf(communication)),
                        Status = false,
                        Description = ""

                    };

                    _context.CompanyCommunications.Add(companyCommunication);
                }

                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = HttpStatusCode.OK, Data = "Company and Communications Linked Successfully!" });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CompanyManagement model)
        {
            try
            {
                if (model is null)
                {
                    return BadRequest("Owner object is null");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model object");
                }

                var entity = await _context.CompanyManagements.FirstOrDefaultAsync(v => v.CompanyId == model.CompanyId);

                if (entity is null)
                {
                    return NotFound();
                }

                entity.CompanyName = model.CompanyName;
                entity.Location = model.Location;
                entity.LinkedInProfile = model.LinkedInProfile;
                entity.Emails = model.Emails;
                entity.PhoneNumbers = model.PhoneNumbers;
                entity.Comments = model.Comments;
                entity.CommunicationPeriodicity = model.CommunicationPeriodicity;

                _context.CompanyManagements.Update(entity);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = HttpStatusCode.OK, Data = "Record Updated!" });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var Entity = await _context.CompanyManagements.Where(v => v.CompanyId == id).AsNoTracking().FirstOrDefaultAsync();
                if (Entity is null)
                {
                    return NotFound();
                }
                _context.CompanyManagements.Remove(Entity);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = HttpStatusCode.OK, Data = "Record Deleted!" });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }

    }
}
