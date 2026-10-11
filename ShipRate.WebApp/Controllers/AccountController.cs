using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Messaging;
using ShipRate.Application.Security;
using ShipRate.Domain.Entities.Security;
using ShipRate.Web.Models;
using ShipRate.WebApp.ViewModels;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Security.Cryptography;
using System.Text.Json;

namespace ShipRate.Web.Controllers
{
    public class AccountController : Controller
    {
        private const string CaptchaSessionKey = "ShipRateCaptchaCode";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuditService _auditService;
        private readonly ISecurityEventService _securityEventService;
        private readonly IPermissionService _permissionService;
        private readonly IUserClaimService _userClaimService;
        private readonly IOtpService _otpService;
        private readonly ISmsService _smsService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuditService auditService, ISecurityEventService securityEventService, IPermissionService permissionService, IUserClaimService userClaimService, IOtpService otpService, ISmsService smsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _auditService = auditService;
            _securityEventService = securityEventService;
            _permissionService = permissionService;
            _userClaimService = userClaimService;
            _otpService = otpService;
            _smsService = smsService;
        }

        // ========== GET: Login ==========
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            GenerateCaptcha();
            return View();
        }


        //14060602 OTP Begin

        // ==========================================
        // 1) متد لاگین اصلاح شده با پشتیبانی از 2FA
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["ReturnUrl"] = model.ReturnUrl;
            var username = model.Username?.Trim();

            // 1) Validate captcha
            var sessionCaptcha = HttpContext.Session.GetString(CaptchaSessionKey);
            if (string.IsNullOrWhiteSpace(model.CaptchaCode) ||
                string.IsNullOrWhiteSpace(sessionCaptcha) ||
                !string.Equals(model.CaptchaCode, sessionCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.CaptchaCode), "Captcha is incorrect");
            }

            var timeStr = HttpContext.Session.GetString("LoginCaptchaTime");
            if (DateTime.TryParse(timeStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var createdAt))
            {
                if (DateTime.Now - createdAt > TimeSpan.FromMinutes(2))
                {
                    ModelState.AddModelError(nameof(model.CaptchaCode), "Captcha expired. Please refresh.");
                }
            }
            else
            {
                ModelState.AddModelError(nameof(model.CaptchaCode), "Captcha expired. Please refresh.");
            }

            // 2) Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Username and password are required.");
            }

            if (!ModelState.IsValid)
            {
                HttpContext.Session.Remove(CaptchaSessionKey);
                HttpContext.Session.Remove("LoginCaptchaTime");
                GenerateCaptcha();
                return View(new LoginViewModel { Username = username, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
            }

            // 3) Find user by username OR email
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(username);
            }

            if (user == null)
            {
                await _securityEventService.LogFailedLoginAsync(username, "User not found");
                ModelState.AddModelError(string.Empty, "Incorrect username/email or password.");
                GenerateCaptcha();
                return View(new LoginViewModel { Username = username, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
            }

            // 4) Check lockout before sign-in
            if (await _userManager.IsLockedOutAsync(user))
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? username, "Account is locked");
                ModelState.AddModelError(string.Empty, "Account is locked. Please try again later.");
                GenerateCaptcha();
                return View(new LoginViewModel { Username = username, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
            }

            // 5) Check InActive
            if (!user.IsActive)
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? username, "Account is InActive");
                ModelState.AddModelError(string.Empty, "Account is InActive. Please contact Administrator.");
                GenerateCaptcha();
                return View(new LoginViewModel { Username = username, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
            }

            // Captcha is valid
            HttpContext.Session.Remove(CaptchaSessionKey);
            HttpContext.Session.Remove("LoginCaptchaTime");

            // 6) بررسی اعتبار رمز عبور بدون ایجاد Session لاگین قطعی
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // ----------------------------------------------------
                // الف) اگر کاربر احراز هویت دو مرحله‌ای (SMS) فعال دارد:
                // ----------------------------------------------------
                if (user.TwoFactorEnabled && user.TwoFactorMethodId == (int)TwoFactorMethodEnum.Sms)
                {
                    if (string.IsNullOrWhiteSpace(user.Mobile))
                    {
                        ModelState.AddModelError(string.Empty, "Mobile number is not configured for Two-Factor Authentication. Please contact administrator.");
                        GenerateCaptcha();
                        return View(new LoginViewModel { Username = username, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
                    }


                    HttpContext.Session.SetString(
                        "PendingTwoFactorUserId",
                        user.Id.ToString());




                    // تولید و ارسال کد OTP (با در نظر گرفتن کول‌داون)
                    var (canResend, _) = await _otpService.CanResendOtpAsync(user.Id, TwoFactorMethodEnum.Sms);
                    if (canResend)
                    {
                        var rawOtp = await _otpService.GenerateAndSaveOtpAsync(user.Id, TwoFactorMethodEnum.Sms, expiryMinutes: 2);
                        await _smsService.SendOtpSmsAsync(user.Mobile, rawOtp);
                    }

                    // هدایت به صفحه اعتبارسنجی OTP
                    return RedirectToAction(nameof(VerifyOtp), new
                    {
                        userId = user.Id,
                        rememberMe = model.RememberMe,
                        returnUrl = model.ReturnUrl
                    });
                }



                // ----------------------------------------------------
                // ب) Two-Factor Authentication با Google Authenticator
                // ----------------------------------------------------
                var isGoogleAuthenticator = user.TwoFactorMethodId == (int)TwoFactorMethodEnum.GoogleAuthenticator;
                if (isGoogleAuthenticator)
                {
                    var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
                    var hasAuthenticatorKey = !string.IsNullOrWhiteSpace(authenticatorKey);
                    if (!user.TwoFactorEnabled || !hasAuthenticatorKey)
                    {
                        return RedirectToAction(
                            nameof(SetupAuthenticator),
                            new
                            {
                                userId = user.Id,
                                returnUrl = model.ReturnUrl
                            });
                    }
                    if (user.TwoFactorEnabled &&
                                       user.TwoFactorMethodId ==
                                       (int)TwoFactorMethodEnum.GoogleAuthenticator)
                    {
                        HttpContext.Session.SetString(
                            "PendingTwoFactorUserId",
                            user.Id.ToString());

                        return RedirectToAction(
                            nameof(VerifyAuthenticator),
                            new
                            {
                                userId = user.Id,
                                rememberMe = model.RememberMe,
                                returnUrl = model.ReturnUrl
                            });
                    }
                }














                if (user.TwoFactorEnabled &&
                    user.TwoFactorMethodId ==
                    (int)TwoFactorMethodEnum.GoogleAuthenticator)
                {
                    HttpContext.Session.SetString(
                        "PendingTwoFactorUserId",
                        user.Id.ToString());

                    return RedirectToAction(
                        nameof(VerifyAuthenticator),
                        new
                        {
                            userId = user.Id,
                            rememberMe = model.RememberMe,
                            returnUrl = model.ReturnUrl
                        });
                }










                // ----------------------------------------------------
                // ب) اگر 2FA فعال نبود -> ورود مستقیم
                // ----------------------------------------------------

                HttpContext.Session.Remove("PendingTwoFactorUserId");
                await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

                user.LastLoginAt = DateTime.Now;
                await _userManager.UpdateAsync(user);

                await _securityEventService.LogSuccessfulLoginAsync(user.Id);
                await _auditService.LogAsync(action: "Login", entityName: "Account");

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Home");
            }

            // 7) سناریوهای شکست در رمز عبور یا قفل شدن
            if (result.IsLockedOut)
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? username, "Account locked due to failed attempts");
                ModelState.AddModelError(string.Empty, "Account has been locked due to multiple failed attempts. Please try again later.");
            }
            else
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? username, "Incorrect username/email or password");
                ModelState.AddModelError(string.Empty, "Incorrect username/email or password.");
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var failedCount = await _securityEventService.GetRecentFailedLoginsCountAsync(user.Email);
                if (failedCount >= 3)
                {
                    ModelState.AddModelError(string.Empty,
                        $"Warning: {failedCount} failed login attempts. Account will be locked after 5 attempts.");
                }
            }

            GenerateCaptcha();
            return View(new LoginViewModel { Username = username, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }


        // ==========================================
        // 2) اکشن GET برای نمایش فرم ورود کد OTP
        // ==========================================
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp(Guid userId, bool rememberMe, string? returnUrl)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || !user.IsActive || user.IsDeleted)
            {
                return RedirectToAction(nameof(Login));
            }

            var (_, remainingSeconds) = await _otpService.CanResendOtpAsync(user.Id, TwoFactorMethodEnum.Sms);

            var vm = new VerifyTwoFactorVm
            {
                UserId = user.Id,
                RememberMe = rememberMe,
                ReturnUrl = returnUrl,
                MaskedMobile = MaskMobile(user.Mobile),
                ResendSeconds = remainingSeconds
            };

            return View(vm);
        }


        // ==========================================
        // 3) اکشن POST برای تأیید نهایی کد OTP
        // ==========================================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyTwoFactorVm model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null || !user.IsActive || user.IsDeleted)
            {
                return RedirectToAction(nameof(Login));
            }

            if (!ModelState.IsValid)
            {
                var (_, remaining) = await _otpService.CanResendOtpAsync(user.Id, TwoFactorMethodEnum.Sms);
                model.ResendSeconds = remaining;
                model.MaskedMobile = MaskMobile(user.Mobile);
                return View(model);
            }

            var verifyResult = await _otpService.VerifyOtpAsync(user.Id, model.Code, TwoFactorMethodEnum.Sms);

            if (!verifyResult.IsSuccess)
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? user.UserName!, $"Invalid 2FA OTP: {verifyResult.ErrorMessage}");
                ModelState.AddModelError(nameof(model.Code), verifyResult.ErrorMessage ?? "Invalid verification code.");

                var (_, remaining) = await _otpService.CanResendOtpAsync(user.Id, TwoFactorMethodEnum.Sms);
                model.ResendSeconds = remaining;
                model.MaskedMobile = MaskMobile(user.Mobile);
                return View(model);
            }

            // ---------------------------------------------
            // ورود موفقیت‌آمیز نهایی به همراه ثبت لاگ‌ها
            // ---------------------------------------------
            await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

            user.LastLoginAt = DateTime.Now;
            await _userManager.UpdateAsync(user);

            await _securityEventService.LogSuccessfulLoginAsync(user.Id);
            await _auditService.LogAsync(action: "2FA Login", entityName: "Account");

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }


        // ==========================================
        // 4) اکشن ارسال مجدد کد (AJAX Resend)
        // ==========================================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || !user.IsActive || user.IsDeleted || string.IsNullOrWhiteSpace(user.Mobile))
            {
                return Json(new { success = false, message = "Invalid user state." });
            }

            var (canResend, remainingSeconds) = await _otpService.CanResendOtpAsync(user.Id, TwoFactorMethodEnum.Sms);
            if (!canResend)
            {
                return Json(new { success = false, message = $"Please wait {remainingSeconds} seconds before requesting a new code.", remainingSeconds });
            }

            var rawOtp = await _otpService.GenerateAndSaveOtpAsync(user.Id, TwoFactorMethodEnum.Sms, expiryMinutes: 2);
            await _smsService.SendOtpSmsAsync(user.Mobile, rawOtp);

            return Json(new { success = true, message = "A new verification code has been sent.", remainingSeconds = 120 });
        }
        // ==========================================
        // متد کمکی برای ماسک کردن شماره موبایل
        // ==========================================
        private static string? MaskMobile(string? mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < 7)
                return mobile;

            return mobile[..4] + "***" + mobile[^3..];
        }
        //14060602 OTP End


































        [HttpGet]
        public IActionResult RefreshCaptcha()
        {
            var code = GenerateCaptcha();
            return Json(new { captcha = code });
        }

        private string GenerateCaptcha()
        {
            var number = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            HttpContext.Session.SetString(CaptchaSessionKey, number);
            ViewBag.CaptchaCode = number;
            return number;
        }

        private string GenerateCaptchaCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        }


        private byte[] GenerateCaptchaImage(string code)
        {
            int width = 160;
            int height = 70;

            using Bitmap bitmap = new Bitmap(width, height);
            using Graphics graphics = Graphics.FromImage(bitmap);
            using MemoryStream ms = new MemoryStream();

            Random random = new Random();

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // ۱. ایجاد پس‌زمینه گرادینت آبی آسمانی ملایم
            using LinearGradientBrush backgroundBrush = new LinearGradientBrush(
                new Rectangle(0, 0, width, height),
                Color.FromArgb(215, 235, 255), // آبی بسیار روشن
                Color.FromArgb(175, 215, 250), // آبی آسمانی
                LinearGradientMode.ForwardDiagonal
            );
            graphics.FillRectangle(backgroundBrush, 0, 0, width, height);

            // ۲. افزودن نویزهای ظریف (نقاط نیمه‌شفاف)
            for (int i = 0; i < 250; i++)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);
                int size = random.Next(1, 3);
                using SolidBrush noiseBrush = new SolidBrush(Color.FromArgb(random.Next(50, 150), 100, 150, 200));
                graphics.FillEllipse(noiseBrush, x, y, size, size);
            }

            // ۳. تنظیمات فونت (کمی کوچک‌تر برای ظاهر ظریف‌تر)
            string[] fonts = { "Arial", "Verdana", "Segoe UI", "Tahoma" };
            int startX = 15;

            for (int i = 0; i < code.Length; i++)
            {
                string digit = code[i].ToString();
                string fontName = fonts[random.Next(fonts.Length)];

                // اندازه فونت کوچک‌تر شده (بازه 18 تا 24)
                using Font font = new Font(fontName, random.Next(18, 24), FontStyle.Bold);

                // رنگ متن: سرمه‌ای تیره برای کنتراست با آبی آسمانی
                using SolidBrush textBrush = new SolidBrush(
                    Color.FromArgb(random.Next(20, 50), random.Next(40, 70), random.Next(80, 120))
                );

                float x = startX + i * 22;
                float y = random.Next(12, 22);

                // اعمال چرخش ملایم برای امنیت
                graphics.TranslateTransform(x, y);
                graphics.RotateTransform(random.Next(-10, 11));
                graphics.DrawString(digit, font, textBrush, 0, 0);
                graphics.ResetTransform();
            }

            // ۴. رسم چند خط منحنی بسیار ملایم (S-Curve) به عنوان نویز پس‌زمینه
            using Pen wavePen = new Pen(Color.FromArgb(80, 255, 255, 255), 1);
            for (int i = 0; i < 3; i++)
            {
                Point[] points = new Point[4];
                for (int j = 0; j < 4; j++)
                    points[j] = new Point(random.Next(0, width), random.Next(0, height));
                graphics.DrawCurve(wavePen, points);
            }

            // ذخیره در سشن (همان منطق قبلی شما)
            HttpContext.Session.SetString("LoginCaptchaCode", code);
            HttpContext.Session.SetString("LoginCaptchaTime", DateTime.Now.ToString("O"));

            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }


        [HttpGet]
        public IActionResult CaptchaImage()
        {
            string code = GenerateCaptchaCode();

            HttpContext.Session.SetString(CaptchaSessionKey, code);

            byte[] imageBytes = GenerateCaptchaImage(code);

            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return File(imageBytes, "image/png");
        }




        // ========== GET: Register ==========
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // ========== POST: Register ==========
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string email, string password, string fullName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "All fields are required!!!!!");
                return View();
            }

            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }

        // ========== POST: Logout ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ========== GET: AccessDenied ==========
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewBag.DialogTitle = "Change Password"; // برای مودال
            return PartialView("_ChangePassword", new ChangePasswordVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVm model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChangePassword", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                // بروزرسانی کوکی امنیتی برای جلوگیری از لاگ‌اوت شدن کاربر
                await _signInManager.RefreshSignInAsync(user);
                var triggerData = new
                {
                    showToast = new
                    {
                        message = "Password changed successfully.",
                        type = "success"
                    },
                    closeModal = new { }
                };

                Response.Headers["HX-Trigger"] =
                    JsonSerializer.Serialize(triggerData);

                return NoContent();
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return PartialView("_ChangePassword", model);
        }


        [HttpGet]
        public async Task<IActionResult> SetupAuthenticator(
            Guid userId,
            string? returnUrl = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _otpService.GetGoogleAuthenticatorSetupAsync(
                userId,
                accountName: null,
                cancellationToken);

            var vm = new GoogleAuthenticatorSetupVm
            {
                UserId = userId,
                SharedKey = result.SharedKey,
                AuthenticatorUri = result.AuthenticatorUri,
                ReturnUrl = returnUrl
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(
            GoogleAuthenticatorSetupVm model,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                // در صورت خطای ولیدیشن، مجدداً SetupData را دریافت می‌کنیم تا QR و Key در View خالی نشوند
                var setup = await _otpService.GetGoogleAuthenticatorSetupAsync(
                    model.UserId,
                    accountName: null,
                    cancellationToken);

                model.SharedKey = setup.SharedKey;
                model.AuthenticatorUri = setup.AuthenticatorUri;

                return View("SetupAuthenticator", model);
            }

            // ۱. اعتبارسنجی کد ارسالی با کلید ثبت‌شده در دیتابیس
            var result = await _otpService.VerifyGoogleAuthenticatorCodeAsync(
                model.UserId,
                model.VerificationCode,
                cancellationToken);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    nameof(model.VerificationCode),
                    result.ErrorMessage ?? "Invalid verification code.");

                var setup = await _otpService.GetGoogleAuthenticatorSetupAsync(
                    model.UserId,
                    accountName: null,
                    cancellationToken);

                model.SharedKey = setup.SharedKey;
                model.AuthenticatorUri = setup.AuthenticatorUri;

                return View("SetupAuthenticator", model);
            }

            // ۲. یافتن کاربر
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "User was not found.");
                return View("SetupAuthenticator", model);
            }

            // ۳. فعال‌سازی رسمی دو مرحله‌ای پس از اولین تایید موفق
            user.TwoFactorEnabled = true;
            user.TwoFactorMethodId = (int)TwoFactorMethodEnum.GoogleAuthenticator;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var setup = await _otpService.GetGoogleAuthenticatorSetupAsync(
                    model.UserId,
                    accountName: null,
                    cancellationToken);

                model.SharedKey = setup.SharedKey;
                model.AuthenticatorUri = setup.AuthenticatorUri;

                return View("SetupAuthenticator", model);
            }

            TempData["SuccessMessage"] = "Google Authenticator has been enabled successfully.";

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult VerifyAuthenticator(
            Guid userId,
            bool rememberMe = false,
            string? returnUrl = null)
        {
            var model = new VerifyAuthenticatorVm
            {
                UserId = userId,
                RememberMe = rememberMe,
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyAuthenticator(
            VerifyAuthenticatorVm model,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ۱. بررسی Session برای اطمینان از اینکه کاربر مرحله پسورد را طی کرده است
            var pendingUserIdStr = HttpContext.Session.GetString("PendingTwoFactorUserId");
            if (string.IsNullOrWhiteSpace(pendingUserIdStr) ||
                !Guid.TryParse(pendingUserIdStr, out var pendingUserId) ||
                pendingUserId != model.UserId)
            {
                ModelState.AddModelError(string.Empty, "Authentication session expired or invalid. Please login again.");
                return View(model);
            }

            // ۲. یافتن کاربر
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "User was not found.");
                return View(model);
            }

            // ۳. بررسی فعال بودن حساب
            if (!user.IsActive || user.IsDeleted)
            {
                await _securityEventService.LogFailedLoginAsync(
                    user.Email ?? user.UserName ?? user.Id.ToString(),
                    "Account is inactive or deleted");

                ModelState.AddModelError(
                    string.Empty,
                    "Account is inactive. Please contact administrator.");

                return View(model);
            }


            // ۴. بررسی قفل نبودن حساب
            if (await _userManager.IsLockedOutAsync(user))
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? user.UserName ?? user.Id.ToString(), "Account is locked");
                ModelState.AddModelError(string.Empty, "Account is locked. Please try again later.");
                return View(model);
            }

            // ۵. بررسی وضعیت فعال بودن Google Authenticator
            if (!user.TwoFactorEnabled || user.TwoFactorMethodId != (int)TwoFactorMethodEnum.GoogleAuthenticator)
            {
                ModelState.AddModelError(string.Empty, "Google Authenticator is not configured for this account.");
                return View(model);
            }

            // ۶. اعتبارسنجی کد ارسالی با سرویس
            var verifyResult = await _otpService.VerifyGoogleAuthenticatorCodeAsync(
                user.Id,
                model.Code,
                cancellationToken);

            if (!verifyResult.IsSuccess)
            {
                // ثبت لاگ تلاش ناموفق
                await _securityEventService.LogFailedLoginAsync(
                    user.Email ?? user.UserName ?? user.Id.ToString(),
                    "Invalid Google Authenticator code");

                ModelState.AddModelError(
                    nameof(model.Code),
                    verifyResult.ErrorMessage ?? "Invalid authenticator code.");

                return View(model);
            }

            // ۷. پاک‌سازی Session موقت احراز هویت
            HttpContext.Session.Remove("PendingTwoFactorUserId");

            // ۸. ورود نهایی کاربر (Sign-in)
            await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

            user.LastLoginAt = DateTime.Now;
            await _userManager.UpdateAsync(user);

            // ۹. لاگ موفقیت و Audit
            await _securityEventService.LogSuccessfulLoginAsync(user.Id);
            await _auditService.LogAsync(action: "Login", entityName: "Account");

            // ۱۰. ریدایرکت
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }

}