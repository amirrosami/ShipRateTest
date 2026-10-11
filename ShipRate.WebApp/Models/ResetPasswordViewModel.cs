using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.Models
{
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "New Password is Required")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Two Passwords are not same!!!!")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
