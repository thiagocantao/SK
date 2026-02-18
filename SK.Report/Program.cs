using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SK.Report.Services;
using SK.Report.Data;
using SK.Report.Extensions;
using System.Globalization;
using SK.Report.Middlewares;
//using Microsoft.ApplicationInsights.AspNetCore.Extensions;
//using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using DevExpress.AspNetCore.Reporting;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(prefix: "SKREPORT_");

IFileProvider? fileProvider = builder.Environment.ContentRootFileProvider;
IConfiguration? configuration = builder.Configuration;

builder.Services.AddDetection();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(2);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<DatabaseSettingsService>();
builder.Services.AddScoped<ApiIntegrationService>();
builder.Services.AddScoped<SessionDataStorageService>();
builder.Services.AddDbContext<ReportContext>(options => options.UseNpgsql(
    configuration.GetConnectionString("SKConnectionString")));
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(
    configuration.GetConnectionString("SqliteConnectionString")!), ServiceLifetime.Transient);

//var options = new ApplicationInsightsServiceOptions { ConnectionString = configuration["ApplicationInsights:ConnectionString"] };
//builder.Services.AddApplicationInsightsTelemetry(options: options);

//builder.Logging.AddApplicationInsights(
//        configureTelemetryConfiguration: (config) =>
//            config.ConnectionString = configuration["ApplicationInsights:ConnectionString"],
//            configureApplicationInsightsLoggerOptions: (options) => { }
//    );

//builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("traces", LogLevel.Trace);

builder.Services.ConfigureReportingServices(builder => {
    builder.UseDevelopmentMode(options => {
        options.EnableClientSideDevelopmentMode = false;
        options.CheckClientLibraryVersions = false;
    });
});

builder.Services.AddDevExpressControls();
builder.Services.AddDashboardConfigurator();
builder.Services.AddReportDesignerConfigurator();
builder.Services.AddMvc();

var cultureInfo = CultureInfo.CreateSpecificCulture("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("pt-BR"),
        new CultureInfo("pt-PT"),
        new CultureInfo("en-US"),
        new CultureInfo("es-ES")
    };
    options.DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var serviceScope = app.Services?.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope?.ServiceProvider.GetRequiredService<AppDbContext>();
    if (context != null)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }
}

var locOptions = app.Services?.GetRequiredService<IOptions<RequestLocalizationOptions>>();
if (locOptions != null)
    app.UseRequestLocalization(locOptions.Value);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDevExpressControls();
app.UseDetection();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.UseSessionDataStorage();
app.MapRazorPages();
app.MapDashboardRoute("api/dashboard", "DefaultDashboard");
app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();
