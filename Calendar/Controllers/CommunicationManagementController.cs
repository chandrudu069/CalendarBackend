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
    public class CommunicationManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;

        public CommunicationManagementController(ApplicationDbContext context)
        {
            _context = context;
            _pagination = new Pagination();

        }

  
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                
                var data = await _context.CommunicationManagements
                                        .OrderBy(c => c.Sequence) 
                                        .ToListAsync();

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
                var data = await _context.CommunicationManagements.ToListAsync();

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
        public async Task<IActionResult> Create([FromBody] CommunicationManagement model)
        {
            try
            {
                if (model is null)
                {
                    return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = "Check the Payload!" });
                }
                if (!ModelState.IsValid)
                {
                    return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = "Validation Error!" });
                }
                model.CommunicationId = Guid.NewGuid();
                _context.CommunicationManagements.Add(model);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = HttpStatusCode.OK, Data = "Record Created!" });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CommunicationManagement model)
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

                var entity = await _context.CommunicationManagements.FirstOrDefaultAsync(v => v.CommunicationId == model.CommunicationId);

                if (entity is null)
                {
                    return NotFound();
                }

                entity.CommunicationName = model.CommunicationName;
                entity.Description = model.Description;
                entity.Sequence = model.Sequence;
                entity.Mandatory = model.Mandatory;

                _context.CommunicationManagements.Update(entity);
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
                var Entity = await _context.CommunicationManagements.Where(v => v.CommunicationId == id).AsNoTracking().FirstOrDefaultAsync();
                if (Entity is null)
                {
                    return NotFound();
                }
                _context.CommunicationManagements.Remove(Entity);
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
