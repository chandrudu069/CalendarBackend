
﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;


namespace Calendar.Models
{
    public class User : IdentityUser
    {
        [Column(TypeName = "varchar(45)")]
        public string? Name { get; set; }
        
        public bool? IsActive { get; set; }
        public DateTime LastUpdated { get; set; }
    }
    public class UserProfile
    {
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
       
        public bool? IsActive { get; set; }
    }
}