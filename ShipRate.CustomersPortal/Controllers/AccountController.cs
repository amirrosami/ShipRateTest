using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.WebAppCustomers.ViewModels;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Security.Cryptography;

namespace ShipRate.WebAppCustomers.Controllers
{
    public class AccountController : Controller
    {
        private const string CaptchaSessionKey = "ShipRateCaptchaCode";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuditService _auditService;
        private readonly ISecurityEventService _securityEventService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuditService auditService, ISecurityEventService securityEventService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _auditService = auditService;
            _securityEventService = securityEventService;
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

        // ========== POST: Login ==========
        [HttpPost]
        [AllowAnonymous]
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
                return View(new LoginViewModel
                {
                    Username = username
                });
            }

            // 3) Find user by username OR email
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(username);
            }

            if (user == null)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers.UserAgent.ToString();
                await _securityEventService.LogFailedLoginAsync(username, "User not found");
                ModelState.AddModelError(string.Empty, "Incorrect username/email or password.");
                GenerateCaptcha();

                return View(new LoginViewModel
                {
                    Username = username
                });
            }

            // 4) Check lockout before sign-in
            if (await _userManager.IsLockedOutAsync(user))
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? username, "Account is locked");
                ModelState.AddModelError(string.Empty, "Account is locked. Please try again later.");
                GenerateCaptcha();

                return View(new LoginViewModel
                {
                    Username = username
                });
            }


            // 5) Check InActive
            if (user.IsActive == false)
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? username, "Account is InActive");
                ModelState.AddModelError(string.Empty, "Account is InActive. Please contact Administrator.");
                GenerateCaptcha();
                return View(new LoginViewModel
                {
                    Username = username
                });
            }





            // Captcha is valid and we are going to attempt login
            HttpContext.Session.Remove(CaptchaSessionKey);
            HttpContext.Session.Remove("LoginCaptchaTime");

            // 5) Important: sign in with actual username
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Sync claims only after successful login
                //await _userClaimService.RefreshUserPermissionClaimsAsync(user);   14050329====> deleted due to customuserclaimsprincipalfactory

                user.LastLoginAt = DateTime.Now;
                await _userManager.UpdateAsync(user);

                await _securityEventService.LogSuccessfulLoginAsync(user.Id);
                await _auditService.LogAsync(action: "Login", entityName: "Account");

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Dashboard");
            }

            if (result.IsLockedOut)
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? username, "Account locked due to failed attempts");
                ModelState.AddModelError(string.Empty, "Incorrect username/email or password.");
            }
            else
            {
                await _securityEventService.LogFailedLoginAsync(user.Email ?? username, "Incorrect username/email or password");
                ModelState.AddModelError(string.Empty, "Incorrect username/email or password");
            }

            // 6) Optional warning for failed attempts
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

            return View(new LoginViewModel
            {
                Username = username
            });
        }
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
            const int width = 160;
            const int height = 70;

            using Bitmap bitmap = new Bitmap(width, height);
            using Graphics graphics = Graphics.FromImage(bitmap);
            using MemoryStream ms = new MemoryStream();
            Random random = new Random();

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(Color.White);

            using (LinearGradientBrush backgroundBrush = new LinearGradientBrush(
                new Rectangle(0, 0, width, height),
                Color.FromArgb(225, 240, 255),
                Color.FromArgb(190, 225, 252),
                LinearGradientMode.ForwardDiagonal))
            {
                graphics.FillRectangle(backgroundBrush, 0, 0, width, height);
            }

            // Base noise
            for (int i = 0; i < 260; i++)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);
                int size = random.Next(1, 3);

                using SolidBrush noiseBrush = new SolidBrush(
                    Color.FromArgb(
                        random.Next(35, 95),
                        random.Next(115, 175),
                        random.Next(155, 225),
                        random.Next(190, 245)));

                graphics.FillEllipse(noiseBrush, x, y, size, size);
            }

            string[] fonts = { "Arial", "Verdana", "Segoe UI", "Tahoma", "Calibri" };
            int digitCount = code.Length;
            int paddingX = 10;
            float slotWidth = (width - (paddingX * 2f)) / digitCount;

            for (int i = 0; i < digitCount; i++)
            {
                string digit = code[i].ToString();
                string fontName = fonts[random.Next(fonts.Length)];
                float fontSize = random.Next(23, 28);

                using Font font = new Font(fontName, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                using StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                float slotLeft = paddingX + (i * slotWidth);
                float slotTop = 8;
                float slotHeight = height - 16;

                // Small wave offset so the digits don't sit on a straight line
                float wave = (float)(Math.Sin((i * 1.2) + random.NextDouble()) * 4.0);
                float jitterY = random.Next(-2, 3);

                RectangleF slotRect = new RectangleF(
                    slotLeft,
                    slotTop + wave + jitterY,
                    slotWidth,
                    slotHeight
                );

                using GraphicsPath path = new GraphicsPath();
                path.AddString(
                    digit,
                    font.FontFamily,
                    (int)font.Style,
                    font.Size,
                    RectangleF.Inflate(slotRect, -2, -2),
                    format);

                RectangleF bounds = path.GetBounds();
                PointF center = new PointF(
                    bounds.Left + bounds.Width / 2f,
                    bounds.Top + bounds.Height / 2f);

                using (Matrix matrix = new Matrix())
                {
                    matrix.Shear(
                        (float)(random.NextDouble() * 0.12 - 0.06),
                        (float)(random.NextDouble() * 0.06 - 0.03));

                    matrix.RotateAt(random.Next(-12, 13), center);
                    matrix.Translate(random.Next(-2, 3), random.Next(-2, 3));
                    path.Transform(matrix);
                }

                // Soft shadow to reduce clarity a bit
                using (GraphicsPath shadowPath = (GraphicsPath)path.Clone())
                using (Matrix shadowMatrix = new Matrix())
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(55, 255, 255, 255)))
                {
                    shadowMatrix.Translate(1.2f, 1.2f);
                    shadowPath.Transform(shadowMatrix);
                    graphics.FillPath(shadowBrush, shadowPath);
                }

                using SolidBrush textBrush = new SolidBrush(
                    Color.FromArgb(
                        random.Next(18, 40),
                        random.Next(40, 70),
                        random.Next(80, 125)));

                graphics.FillPath(textBrush, path);
            }

            // Disturbance lines crossing the text
            using (Pen linePen = new Pen(Color.FromArgb(130, 70, 130, 200), 1.2f))
            {
                for (int i = 0; i < 4; i++)
                {
                    Point[] points = new Point[5];
                    for (int j = 0; j < points.Length; j++)
                    {
                        points[j] = new Point(
                            j * width / (points.Length - 1),
                            random.Next(8, height - 8));
                    }
                    graphics.DrawCurve(linePen, points);
                }
            }

            // Tiny foreground speckles
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);

                using SolidBrush dotBrush = new SolidBrush(
                    Color.FromArgb(
                        random.Next(100, 160),
                        random.Next(80, 130),
                        random.Next(120, 180),
                        random.Next(150, 220)));

                graphics.FillRectangle(dotBrush, x, y, 1, 1);
            }

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
        [HttpGet]
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
    }
}
