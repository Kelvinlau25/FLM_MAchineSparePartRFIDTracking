using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using FILM_Sparepart_MVC.DAL;
using FILM_Sparepart_MVC.Hubs;
using FILM_Sparepart_MVC.Services;
using FILM_Sparepart_MVC.Repositories;
using FILM_Sparepart_MVC.Repository;
using FILM_Sparepart_MVC.Helper_Code;

var builder = WebApplication.CreateBuilder(args);


// Initialize static configuration helper for legacy model classes
AppConfig.Initialize(builder.Configuration);

// Persist Data Protection keys to disk so sessions and antiforgery tokens
// survive app pool recycles and restarts in IIS
var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetApplicationName("FILM_Sparepart_MVC");

// Add services to the container
builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// Configure Entity Framework Core
builder.Services.AddDbContext<RfidAuditContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLCon")));

builder.Services.AddDbContext<TMS_ITEquiptContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBAccess")));

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddAuthorization();

// Configure Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor for accessing HttpContext in non-controller classes
builder.Services.AddHttpContextAccessor();

// Register RFIDService as a singleton so both the BackgroundService host
// and any controllers that inject it share the SAME running instance
builder.Services.AddSingleton<RFIDService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<RFIDService>());

// Other services
builder.Services.AddScoped<ReaderRFID>();
builder.Services.AddScoped<CommonRepo>();
builder.Services.AddScoped<RfidAuditRepo>();

// Configure SignalR
builder.Services.AddSignalR();

// Configure WebOptimizer for bundling (migrated from BundleConfig.cs)
builder.Services.AddWebOptimizer(pipeline =>
{
    // JavaScript bundles
    pipeline.AddJavaScriptBundle("/bundles/jquery", "js/jquery-3.4.1.js");
    pipeline.AddJavaScriptBundle("/bundles/jqueryval", "js/jquery.validate.js", "js/jquery.validate.unobtrusive.js");
    pipeline.AddJavaScriptBundle("/bundles/modernizr", "js/modernizr-2.8.3.js");
    pipeline.AddJavaScriptBundle("/bundles/bootstrap", "js/bootstrap.js", "js/respond.js");
    pipeline.AddJavaScriptBundle("/bundles/bootstrap-select", "js/bootstrap-select.js", "js/script-bootstrap-select.js");

    // CSS bundles
    pipeline.AddCssBundle("/Content/css", "css/bootstrap.css", "css/Site.css");
    pipeline.AddCssBundle("/Content/Bootstrap-Select/css", "css/style/bootstrap-select.css", "css/style/bootstrap-select.min.css");
});

var app = builder.Build();
app.Logger.LogWarning(">>> PROCESS IS 64-BIT: {Is64}", Environment.Is64BitProcess);
Console.WriteLine($">>> PROCESS IS 64-BIT: {Environment.Is64BitProcess}");
Console.WriteLine($">>> DOTNET_ROOT: {Environment.GetEnvironmentVariable("DOTNET_ROOT") ?? "not set"}");
Console.WriteLine($">>> PID: {Environment.ProcessId}");

// Initialize static DB class with HttpContextAccessor
var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
DBModel.DB.Configure(httpContextAccessor);
DBModel.DB1.Configure(httpContextAccessor);

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Log all unhandled exceptions to stdout (visible in IIS logs)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "[Unhandled Exception] Path: {Path}", context.Request.Path);
        throw;
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseWebOptimizer();

app.UseRouting();

// Session MUST be before Authentication so session is available during auth
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.MapHub<RfidHub>("/rfidHub");

app.Run();