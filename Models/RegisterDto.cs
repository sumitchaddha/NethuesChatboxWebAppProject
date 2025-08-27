﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Nethues_ChatboxWebApp.Models
{
    public class RegisterDto
    {
        public required  string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
