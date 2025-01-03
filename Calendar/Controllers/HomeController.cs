using Calendar.Models;
using Calendar.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AstroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public HomeController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }


        
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserViewModel model)
        {
            // Check if the user already exists
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = "Username already exists!" });

            // Create a new user
            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Name = model.Name,
               
            };


            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = "User creation failed! Please check user details and try again." });


            if (!await _roleManager.RoleExistsAsync(model.RoleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
            }
            await _userManager.AddToRoleAsync(user, model.RoleName);

            return Ok(new
            {
                StatusCode = HttpStatusCode.OK,
                Data = "Account Successfully Created.",
                UserId = user.Id
            });

        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] UserViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (await _userManager.CheckPasswordAsync(user, model.Password))
                {
                  
                    var roles = await _userManager.GetRolesAsync(user);

                    var roleName = roles.FirstOrDefault(); 

                    if (roleName == UserRoles.Admin || roleName == UserRoles.Staff)
                    {
                        return Ok(new
                        {
                            StatusCode = HttpStatusCode.OK,
                            userId = user.Id,
                            roleName = roleName, 
                            name = user.Name,
                        });
                    }
                    else
                    {
                        return Ok(new { StatusCode = HttpStatusCode.Forbidden, Data = "User does not have a valid role!" });
                    }
                }
                else
                {
                    return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = "Please check the credentials" });
                }
            }
            else
            {
                return Ok(new { StatusCode = HttpStatusCode.NotFound, Data = "User does not exist!" });
            }
        }

       
        private string DisplayError(IdentityResult result)
        {
            List<IdentityError> errorList = result.Errors.ToList();
            var errors = string.Join(", ", errorList.Select(e => e.Description));
            return errors;
        }

        // Helper to generate JWT token
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }
    }
}
