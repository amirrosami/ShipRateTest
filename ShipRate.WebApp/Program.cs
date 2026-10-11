using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShipRate.Application.DTOs.ApiIr;
using ShipRate.Application.DTOs.Reports;
using ShipRate.Application.Hubs;
using ShipRate.Application.Identity;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Interfaces.BillOfLadings;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.Interfaces.ContainerActions;
using ShipRate.Application.Interfaces.Dashboard;
using ShipRate.Application.Interfaces.Documents;
using ShipRate.Application.Interfaces.ExternalServices;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Interfaces.Location;
using ShipRate.Application.Interfaces.Logs;
using ShipRate.Application.Interfaces.Manifest;
using ShipRate.Application.Interfaces.Marketing;
using ShipRate.Application.Interfaces.Messaging;
using ShipRate.Application.Interfaces.Notif;
using ShipRate.Application.Interfaces.Reports;
using ShipRate.Application.Interfaces.SalesSpecial;
using ShipRate.Application.Interfaces.Transit;
using ShipRate.Application.Security;
using ShipRate.Application.Services;
using ShipRate.Application.Services.BaseInfo;
using ShipRate.Application.Services.BillOfLadings;
using ShipRate.Application.Services.Cargos;
using ShipRate.Application.Services.ContainerActions;
using ShipRate.Application.Services.Dashboard;
using ShipRate.Application.Services.Documents;
using ShipRate.Application.Services.External;
using ShipRate.Application.Services.Finance;
using ShipRate.Application.Services.JobsServices;
using ShipRate.Application.Services.Location;
using ShipRate.Application.Services.Logs;
using ShipRate.Application.Services.Marketing;
using ShipRate.Application.Services.Messaging;
using ShipRate.Application.Services.Notif;
using ShipRate.Application.Services.Reports;
using ShipRate.Application.Services.SalesSpecial;
using ShipRate.Application.Services.ShipRate.Application.Services;
using ShipRate.Application.Services.SMS;
using ShipRate.Application.Services.Transit;
using ShipRate.Application.Utilities;
using ShipRate.Common.Notifications;
using ShipRate.Common.Notifications.extensions;
using ShipRate.Common.Report;
using ShipRate.Common.Web;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;
using ShipRate.Infrastructure.Filters;
using ShipRate.Infrastructure.Repositories;
using ShipRate.Infrastructure.Repositories.Queries;
using ShipRate.Infrastructure.Repositories.ShipRate.Infrastructure.Repositories;
using ShipRate.Infrastructure.Security;
using ShipRate.Infrastructure.Services;
using ShipRate.WebApp.Configs;
using ShipRate.WebApp.Filters;
using System.Net.Http.Headers;
using static ShipRate.Application.Security.FileSignatureValidator;
using ReportService = ShipRate.Application.Services.Reports.ReportService;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserService>();
builder.Services.AddControllersWithViews();
//.AddViewLocalization(
//    Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix,
//    options => options.ResourcesPath = "Resources");


// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShipRateConnection")
        , sqlServerOptions =>
        {
            //sqlServerOptions.EnableRetryOnFailure(
            //    maxRetryCount: 5,
            //    maxRetryDelay: TimeSpan.FromSeconds(10),
            //    errorNumbersToAdd: null);
        });
    //options.ConfigureWarnings(w =>
    //{
    //    w.Ignore(new EventId(10625)); // Shadow FK warning
    //    w.Ignore(new EventId(10102)); // Shadow FK warning
    //});
});


builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();

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


builder.Services.AddHttpClient("saoi", c =>
{
    c.BaseAddress = new Uri("https://saoi.ir");
    c.DefaultRequestHeaders.Add("User-Agent",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
});
builder.Services.AddScoped<SaoiCurrencyFetchService>();


builder.Services.Configure<ApiIrOptions>(
    builder.Configuration.GetSection("Pasavana"));

builder.Services.AddHttpClient<IApiIrService, ApiIrService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ApiIrOptions>>().Value;

    //client.BaseAddress = new Uri(options.BaseUrl);
    client.BaseAddress = new Uri(@"https://s.api.ir");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", options.BearerToken);
});




builder.Services.Configure<AttachmentSecurityOptions>(
    builder.Configuration.GetSection("AttachmentSecurity"));

builder.Services.AddHttpClient<ICiscoTelephonyService, CiscoTelephonyService>();

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IUserPermissionGroupRepository, UserPermissionGroupRepository>();
builder.Services.AddScoped<IPermissionGroupPermissionRepository, PermissionGroupPermissionRepository>();
builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IMeasurementUnitRepository, MeasurementUnitRepository>();
builder.Services.AddScoped<IRepository<MeasurementUnitCategory>, Repository<MeasurementUnitCategory>>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IBaseDataRepository, BaseDataRepository>();
builder.Services.AddScoped<IBankRepository, BankRepository>();
builder.Services.AddScoped<IVesselRepository, VesselRepository>();
builder.Services.AddScoped<IShippingLineRepository, ShippingLineRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IPortDistanceRepository, PortDistanceRepository>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();






// Register Services
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuditLogExportService, AuditLogExportService>();
builder.Services.AddScoped<ISecurityEventService, SecurityEventService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPermissionManagementService, PermissionManagementService>();
builder.Services.AddScoped<IPermissionGroupService, PermissionGroupService>();
builder.Services.AddScoped<IUserClaimService, UserClaimService>();
builder.Services.AddScoped<AuditActionFilter>();

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();


builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ICargoCommodityService, CargoCommodityService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IPackageTypeService, PackageTypeService>();
builder.Services.AddScoped<IMeasurementUnitService, MeasurementUnitService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IBaseDataService, BaseDataService>();
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IVesselService, VesselService>();
builder.Services.AddScoped<IIndustryService, IndustryService>();
builder.Services.AddScoped<IBlackListReasonService, BlackListReasonService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
//builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPortDistanceService, PortDistanceService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJobLookupService, JobLookupService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IPaymentDocumentService, PaymentDocumentService>();
builder.Services.AddScoped<IEventTypeService, EventTypeService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IRequestContext, RequestContext>();
builder.Services.AddScoped<IInvoiceAppService, InvoiceAppService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITariffService, TariffService>();
builder.Services.AddScoped<IContainerActionService, ContainerActionService>();
builder.Services.AddScoped<ITIRCarnetService, TIRCarnetService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IEDIService, EDIService>();
builder.Services.AddScoped<ITipService, TipService>();
builder.Services.AddScoped<IBLSerialSeriesService, BLSerialSeriesService>();
builder.Services.AddScoped<ITelexReleaseService, TelexReleaseService>();
builder.Services.AddScoped<ICargoDetailsService, CargoDetailsService>();
builder.Services.AddScoped<IPackagesService, PackagesService>();
builder.Services.AddScoped<IJobPartyService, JobPartyService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IBLService, BLService>();
builder.Services.AddScoped<ITreasuryDocumentService, TreasuryDocumentService>();
builder.Services.AddScoped<ISettlementService, SettlementService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IRazorViewRenderService, RazorViewRenderService>();
builder.Services.AddScoped<IRateInquiryEmailService, RateInquiryEmailService>();
builder.Services.AddScoped<ICustomsClearanceService, CustomsClearanceService>();
builder.Services.AddScoped<ICustomsClearanceItemService, CustomsClearanceItemService>();
builder.Services.AddScoped<ICustomsClearanceContainerService, CustomsClearanceContainerService>();
builder.Services.AddScoped<ICustomsClearanceChargeService, CustomsClearanceChargeService>();
builder.Services.AddScoped<ICustomsClearanceTimelineService, CustomsClearanceTimelineService>();
builder.Services.AddScoped<ICustomsClearanceLicenseService, CustomsClearanceLicenseService>();
builder.Services.AddScoped<ICustomsInspectionService, CustomsInspectionService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<ICurrentCompanyService, CurrentCompanyService>();
builder.Services.AddScoped<IVaultService, VaultService>();
builder.Services.AddScoped<IPettyCashService, PettyCashService>();
builder.Services.AddScoped<IAppTaskService, AppTaskService>();
builder.Services.AddScoped<IProductStockTransactionService, ProductStockTransactionService>();
builder.Services.AddScoped<INotificationService, ShipRate.Application.Services.Notif.NotificationService>();
builder.Services.AddScoped<INotificationRecipientResolver, NotificationRecipientResolver>();
builder.Services.AddScoped<INotificationRuleService, ShipRate.Application.Services.Notif.NotificationRuleService>();
builder.Services.AddScoped<IFileMalwareScanner, NoOpFileMalwareScanner>();
builder.Services.AddScoped<IMessagingService, MessagingService>();
builder.Services.AddScoped<IFileSignatureValidator, FileSignatureValidator>();
builder.Services.AddScoped<IAttachmentSecurityService, AttachmentSecurityService>();
builder.Services.AddScoped<IUserTeamService, UserTeamService>();
builder.Services.AddScoped<IPdfExtractionService, PdfExtractionService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ITransportDocumentService, TransportDocumentService>();
builder.Services.AddScoped<IContainerRecommendationService, ContainerRecommendationService>();
builder.Services.AddScoped<IExportDocsReceiptService, ExportDocsReceiptService>();
builder.Services.AddScoped<IBLManagementService, BLManagementService>();
builder.Services.AddScoped<IVoyageManagementService, VoyageManagementService>();
builder.Services.AddScoped<IDeliveryOrderService, DeliveryOrderService>();
builder.Services.AddScoped<IGuaranteePlanService, GuaranteePlanService>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
builder.Services.AddScoped<IDocumentTypeObjectTypeService, DocumentTypeObjectTypeService>();
builder.Services.AddScoped<IAccountCodeService, AccountCodeService>();
builder.Services.AddScoped<IDetentionService, DetentionService>();
builder.Services.AddScoped<FinanceQueries>();
builder.Services.AddScoped<IInvoiceAppService, InvoiceAppService>();

builder.Services.AddScoped<IBlackListService, BlackListService>();
builder.Services.AddScoped<ISalesSpecialService, SalesSpecialService>();
builder.Services.AddScoped<INOAService, NOAService>();
builder.Services.AddScoped<IApiInquiryLogService, ApiInquiryLogService>();
builder.Services.AddScoped<IBLManifestReportService, BLManifestReportService>();
builder.Services.AddScoped<IReportRenderer, FastReportRenderer>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<RateRequestService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<ISmsQueueService, SmsQueueService>();
builder.Services.AddScoped<ISmsAuthService, SmsAuthService>();
builder.Services.AddScoped<ISmsManagementService, SmsManagementService>();
builder.Services.AddScoped<IContainerService, ContainerService>();
builder.Services.AddScoped<IImageUtility, ImageUtility>();
builder.Services.AddScoped<IMarketingCallService, MarketingCallService>();

var notifConfig = builder.Configuration.GetSection("Notification").Get<NotificationConfig>();
var notifworkerConfig = builder.Configuration.GetSection("NotificationWorker").Get<NotificationWorkerConfig>();

// add notification service dependencies
builder.Services.AddNotificationServices<NotificationRepository>(notifConfig);
// add notification background process that sends New Nitifs to receivers .
//builder.Services.AddNotificationSenderWorker(notifworkerConfig);

builder.Services.AddReportService<ReportConfigRepository>();
builder.Services.AddScoped<IReportExecutor<TestReportRequest>, TestReport>();
//builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews(options =>
{
    options.ModelMetadataDetailsProviders.Add(
       new CustomMetadataProvider()
   );
    options.Filters.Add<GlobalExceptionActionFilter>();
    options.ModelBinderProviders.Insert(0, new CustomNumericModelBinderProvider());
});


builder.Services.AddCors(o => o.AddPolicy("AllowAllOrigins", builder =>
{
    builder
    .SetIsOriginAllowed((host) => true)
    //.SetIsOriginAllowedToAllowWildcardSubdomains()
    .AllowAnyMethod()
    .AllowCredentials()
    .AllowAnyHeader();
}));



builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make session cookie essential
});



builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddSmsInfrastructure(builder.Configuration);


//builder.Services.AddControllersWithViews();
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    _ = db.Model; // Force model build once at startup
    sw.Stop();
    Console.WriteLine($"Model warmup: {sw.ElapsedMilliseconds}ms (one-time cost at startup)");
}




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
//app.UseRequestLocalization();
app.UseRouting();
app.UseFastReport();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<MessagingHub>("/messagingHub");

app.Run();
