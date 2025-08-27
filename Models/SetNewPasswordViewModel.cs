using System.ComponentModel.DataAnnotations;

namespace Nethues_ChatboxWebApp.Models
{
    public class SetNewPasswordViewModel
    {
        public required string Email { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [Compare("NewPassword", ErrorMessage = "The new and confirm password do not match")]
        public required string ConfirmPassword { get; set; }
    }
}
