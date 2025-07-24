using CloudinaryDotNet;
using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Repository.Interfaces;
using EXE201.Repository.Repositories;
using EXE201.Services.Hubs;
using EXE201.Services.Interfaces;
using EXE201.Services.Models;
using EXE201.Services.Services;
using Google.Api;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serenity_Solution.Seeders;
using Serenity_Solution.Unity;
using static EXE201.Services.Models.OrderInfoModel;


var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//add service and repository
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IChatService, ChatService>();
//builder.Services.AddScoped<IVnPayServicecs, VnPayService>(); //Vnpay
builder.Services.AddSingleton<IVnPayServicecs, VnPayService>();


builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();



builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});


builder.Services.AddSignalR();
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Log ra console
builder.Logging.AddDebug();   // Log vào Debug Window

builder.Services.AddIdentity<User, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = false)
                        .AddEntityFrameworkStores<ApplicationDbContext>()
                        .AddDefaultTokenProviders();

//builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
//{
//    hostingContext.HostingEnvironment.EnvironmentName = "Development";
//});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(14); // giữ đăng nhập 14 ngày
        options.SlidingExpiration = true;
    })
    .AddGoogle(googleOptions =>
 {
     googleOptions.ClientId = builder.Configuration["GoogleKeys:ClientID"];
     googleOptions.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
     googleOptions.SaveTokens = true;
     
 });

void AddGoogle(Action<object> value)
{
    throw new NotImplementedException();
}

builder.Services.AddAuthorization(); // Ensure authorization is added
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Cho HTTPS
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var serviceProvider = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        await IdentitySeeder.SeedRolesAndAdminAsync(serviceProvider);
        //await IdentitySeeder.SeedDataPsychologist(serviceProvider); // them
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapHub<NotificationHub>("/notificationHub");


app.UseRouting();
app.UseSession();//them
app.UseAuthentication();
app.UseAuthorization();
//them
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exceptionHandlerPathFeature?.Error, "Unhandled exception");

        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred.");
    });
});

app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chatHub");
app.Run();
 