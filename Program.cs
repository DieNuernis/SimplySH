using Microsoft.EntityFrameworkCore;
using SimplySH.Data;
using SimplySH.Hubs;
using SimplySH.Models.Auth;
using SimplySH.Models.SSH;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Datenbank laden
builder.Services.AddDbContext<MyDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ).EnableDetailedErrors()
     .EnableSensitiveDataLogging()
);
// Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<MyDBContext>();

// MVC und SignalR-Dienste registrieren
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// Standardroute und SignalR-Hub konfigurieren
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();  // <<< NEU
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Connection}/{action=Index}/{id?}"
);
app.MapHub<SSHHub>("/ssh");
app.MapRazorPages(); // Wichtig für Identity-Seiten!

app.Run();
