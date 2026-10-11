using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Hubs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BillOfLadings;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Interfaces.Messaging;
using ShipRate.Application.Interfaces.Notif;
using ShipRate.Application.Interfaces.SalesSpecial;
using ShipRate.Application.Security;
using ShipRate.Application.Services;
using ShipRate.Application.Services.BillOfLadings;
using ShipRate.Application.Services.Cargos;
using ShipRate.Application.Services.Finance;
using ShipRate.Application.Services.JobsServices;
using ShipRate.Application.Services.Messaging;
using ShipRate.Application.Services.Notif;
using ShipRate.Application.Services.SalesSpecial;
using ShipRate.Common.Web;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;
using ShipRate.Infrastructure.Repositories.Queries;
using System.Globalization;
using static ShipRate.Application.Security.FileSignatureValidator;


// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserService>();


// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShipRateConnection"));
}
    );
// ========== Identity Configuration ==========
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password Settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout Settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User Settings
    options.User.RequireUniqueEmail = true;

    // SignIn
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// ========== Cookie Settings ==========
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services
    .AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var supportedCultures = new[]
{
    new CultureInfo("fa-IR"),
    new CultureInfo("en-US")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("fa-IR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

builder.Services.AddSignalR();

// Register Services
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ISecurityEventService, SecurityEventService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IRazorViewRenderService, RazorViewRenderService>();
builder.Services.AddScoped<IRateInquiryEmailService, RateInquiryEmailService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();

builder.Services.AddScoped<IJobLookupService, JobLookupService>();
builder.Services.AddScoped<ISalesSpecialService, SalesSpecialService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobLookupService, JobLookupService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRuleService, NotificationRuleService>();
builder.Services.AddScoped<ICargoDetailsService, CargoDetailsService>();
builder.Services.AddScoped<IPdfExtractionService, PdfExtractionService>();

builder.Services.AddScoped<IFileMalwareScanner, NoOpFileMalwareScanner>();
builder.Services.AddScoped<IMessagingService, MessagingService>();
builder.Services.AddScoped<IFileSignatureValidator, FileSignatureValidator>();
builder.Services.AddScoped<IAttachmentSecurityService, AttachmentSecurityService>();
builder.Services.AddScoped<IUserTeamService, UserTeamService>();
builder.Services.AddScoped<RateRequestService>();
builder.Services.AddScoped<IRequestContext, RequestContext>();
builder.Services.AddScoped<IInvoiceAppService, InvoiceAppService>();


builder.Services.AddScoped<PaymentDocumentService>();
builder.Services.AddScoped<SettlementService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<FinanceQueries>();
builder.Services.AddScoped<IPaymentDocumentService, PaymentDocumentService>();
builder.Services.AddScoped<ISalesSpecialService, SalesSpecialService>();
//builder.Services.AddScoped<IPaymentDocumentService, PaymentDocumentService>();
builder.Services.AddScoped<ShippingTrackingMvc.Services.IShipmentService, ShippingTrackingMvc.Services.ShipmentService>();


builder.Services.AddScoped<ShipRate.Application.Interfaces.Jobs.IShipmentService, ShipRate.Application.Services.JobsServices.ShipmentService>();
builder.Services.AddScoped<BLService>();
builder.Services.AddScoped<IBLService, BLService>();
builder.Services.AddScoped<CustomerServiceRequestService>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make session cookie essential
});

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(5001, listenOptions =>
//    {
//        listenOptions.UseHttps("aspnetapp.pfx", "Password123");
//    });
//});


builder.Services.AddSignalR();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
//app.UseStaticFiles(); // کشینگ استاتیک فایل‌ها اینجا کنترل می‌شود
//app.UseRouting();
//app.UseRequestLocalization();
//app.UseAuthentication();
//app.UseAuthorization();
//app.UseSession();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Dashboard}/{action=Index}/{id?}");


app.UseHttpsRedirection();


app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (ctx.File.Name.Equals("service-worker.js", StringComparison.OrdinalIgnoreCase))
        {
            ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            ctx.Context.Response.Headers["Pragma"] = "no-cache";
            ctx.Context.Response.Headers["Expires"] = "0";
        }
    }
});


app.UseRouting();

app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();


app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");


app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<MessagingHub>("/messagingHub");
app.Run();