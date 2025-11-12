using IRRRL.Core.Interfaces;
using IRRRL.Core.Services;
using IRRRL.Infrastructure.AI;
using IRRRL.Infrastructure.Data;
using IRRRL.Infrastructure.Documents;
using IRRRL.Infrastructure.Services;
using IRRRL.Web.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/irrrl-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("IRRRL.Infrastructure")
    )
);

// Data Protection (for encrypting SSN and other sensitive data)
// Uses default configuration - keys stored in %LOCALAPPDATA%\ASP.NET\DataProtection-Keys
builder.Services.AddDataProtection();

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure authentication cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// SignalR
builder.Services.AddSignalR();

// Core Services
builder.Services.AddScoped<INetTangibleBenefitCalculator, NetTangibleBenefitCalculator>();
builder.Services.AddScoped<IEligibilityService, EligibilityService>();
builder.Services.AddScoped<IApplicationWorkflowService, ApplicationWorkflowService>();

// Security Services
builder.Services.AddScoped<IDataProtectionService, DataProtectionService>();

// AI Services
var aiConfig = new AIServiceConfig
{
    Endpoint = builder.Configuration["AzureOpenAI:Endpoint"] ?? "",
    ApiKey = builder.Configuration["AzureOpenAI:ApiKey"] ?? "",
    ModelName = builder.Configuration["AzureOpenAI:ModelName"] ?? "gpt-4",
    MaxTokens = int.Parse(builder.Configuration["AzureOpenAI:MaxTokens"] ?? "2000"),
    Temperature = double.Parse(builder.Configuration["AzureOpenAI:Temperature"] ?? "0.7")
};
builder.Services.AddSingleton(aiConfig);
builder.Services.AddScoped<IAIActionItemGenerator, AIActionItemGenerator>();
builder.Services.AddScoped<IAIDocumentValidator, AIDocumentValidator>();

// Document Services
builder.Services.AddScoped<IDocumentGenerator, QuestPDFDocumentGenerator>();

// Infrastructure Services
builder.Services.AddScoped<IApplicationNotificationService, ApplicationNotificationService>();
// ILoanOfficerService removed - now using Vertical Slice Architecture with MediatR

// MediatR for CQRS (Vertical Slice Architecture)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    
    // Add pipeline behaviors (order matters!)
    cfg.AddOpenBehavior(typeof(IRRRL.Web.Features.Common.Behaviors.LoggingBehavior<,>));
});

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VeteranOnly", policy => policy.RequireRole("Veteran"));
    options.AddPolicy("LoanOfficerOnly", policy => policy.RequireRole("LoanOfficer"));
    options.AddPolicy("UnderwriterOnly", policy => policy.RequireRole("Underwriter"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("LoanOfficer", "Underwriter", "Administrator"));
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DbInitializer.InitializeAsync(scope.ServiceProvider);
        Log.Information("Database initialized successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while initializing the database");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapRazorPages();
app.MapBlazorHub();
app.MapHub<ApplicationHub>("/hubs/application");

// Only use fallback for non-Account routes
app.MapFallbackToPage("/_Host");

Log.Information("IRRRL Application starting...");

app.Run();

