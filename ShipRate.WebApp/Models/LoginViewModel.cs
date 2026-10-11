using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.Models
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

    public class VerifyTwoFactorVm
    {
        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Please Enter 5 Digit Code")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Code Length Should Be 5 Digits")]
        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Code Should include Only Digits")]
        public string Code { get; set; } = null!;
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
        public string? MaskedMobile { get; set; }
        public int ResendSeconds { get; set; }
    }


    public class GoogleAuthenticatorSetupVm
    {
        [Required]
        public Guid UserId { get; set; }

        public string? SharedKey { get; set; }

        public string? AuthenticatorUri { get; set; }

        [Required(ErrorMessage = "Please enter the verification code")]
        [StringLength(6, MinimumLength = 6,
            ErrorMessage = "Code must be 6 digits")]
        [RegularExpression("^[0-9]{6}$",
            ErrorMessage = "Code must contain digits only")]
        public string VerificationCode { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public sealed class VerifyAuthenticatorVm
    {
        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Authenticator code is required.")]
        [StringLength(
            6,
            MinimumLength = 6,
            ErrorMessage = "Authenticator code must be 6 digits.")]
        [RegularExpression(
            "^[0-9]{6}$",
            ErrorMessage = "Authenticator code must contain digits only.")]
        public string Code { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }



}
