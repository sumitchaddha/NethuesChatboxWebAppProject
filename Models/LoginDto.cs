﻿using System.ComponentModel.DataAnnotations;

namespace Nethues_ChatboxWebApp.Models
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

    }
}
