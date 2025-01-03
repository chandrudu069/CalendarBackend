using Calendar.Configuration;
using Calendar.Configurationn;
using Calendar.Helpers;
using Calendar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Calendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        
        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _pagination = new Pagination();
           
        }
       
        
      

        [HttpPost("ChangePassword")]
        public ActionResult ChangePassword(string UserId, string OldPassword, string NewPassword)
        {
            if (string.IsNullOrWhiteSpace(OldPassword))
            {
                return BadRequest("OldPassword cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                return BadRequest("NewPassword cannot be empty");
            }

            var entity = _context.Users.FirstOrDefault(s => s.Id == UserId);
            if (entity == null)
            {
                return NotFound(new { StatusCode = HttpStatusCode.NotFound, Data = "User not found" });
            }

            // Blocking ChangePasswordAsync to make it synchronous
            var result = _userManager.ChangePasswordAsync(entity, OldPassword, NewPassword).GetAwaiter().GetResult();

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { StatusCode = HttpStatusCode.BadRequest, Errors = errors });
            }

            return Ok(new { StatusCode = HttpStatusCode.OK, Data = "Password changed successfully" });
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(string Id)
        {

            var entity = await _context.Users.FindAsync(Id);

            if (entity == null)
            {
                return NotFound(new { StatusCode = HttpStatusCode.NotFound, Message = "User not found." });
            }

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Record Deleted" });
        }

        private string DisplayError(IdentityResult result)
        {
            List<IdentityError> errorList = result.Errors.ToList();
            var errors = string.Join(", ", errorList.Select(e => e.Description));
            return errors;
        }
    }
}