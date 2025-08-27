using System.ComponentModel.DataAnnotations;

namespace Nethues_ChatboxWebApp.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string? Email { get; set; }
    }
}
