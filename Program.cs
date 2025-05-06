using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSU_BARODA.Data;
using MSU_BARODA.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure MySQL Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23))));

// Register Email Service
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EmailServiceAdmin>();



// Add MVC and Session Support
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

// Middleware Configuration
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();


// Default Route - Directs to Student Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Admin Route - Directs to Admin Login Page when "/admin" is visited
app.MapControllerRoute(
    name: "admin",
    pattern: "admin",
    defaults: new { controller = "Admin", action = "Login" });

app.Run();
