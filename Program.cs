using Microsoft.EntityFrameworkCore;
using SimplySH.Data;
using SimplySH.Hubs;
using SimplySH.Models.SSH;

var builder = WebApplication.CreateBuilder(args);

// SSH-Konfigurationswerte aus Datenbank laden
builder.Services.AddDbContext<MyDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ).EnableDetailedErrors()
     .EnableSensitiveDataLogging()
);

// MVC und SignalR-Dienste registrieren
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// Standardroute und SignalR-Hub konfigurieren
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Connection}/{action=Index}/{id?}"
);
app.MapHub<SSHHub>("/ssh");

app.Run();
