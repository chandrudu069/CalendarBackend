

﻿using System.ComponentModel.DataAnnotations;

namespace Calendar.Models
{
    public class RegisterRequest
    { 
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string RoleName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}