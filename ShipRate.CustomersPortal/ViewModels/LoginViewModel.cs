using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebAppCustomers.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User name is required")]
        [Display(Name = "User Name")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
        public string? ErrorMessage { get; set; }

        [Required(ErrorMessage = "Captcha is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Captcha must be 6 digits")]
        public string CaptchaCode { get; set; }
    }
}
