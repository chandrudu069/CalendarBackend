using System.ComponentModel.DataAnnotations;

namespace Calendar.Models

{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
        public string? UserId { get; set; }
        public string? Role { get; set; }
        public string? Name { get; set; }
        public bool Status { get; set; }
    }
}
