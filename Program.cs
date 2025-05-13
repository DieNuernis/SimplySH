using Microsoft.EntityFrameworkCore;
using SimplySH.Data;
using SimplySH.Hubs;
using SimplySH.Models.SSH;

var builder = WebApplication.CreateBuilder(args);

// SSH-Konfigurationswerte aus appsettings.json laden
builder.Services.Configure<SSHSettings>(builder.Configuration.GetSection("Ssh"));

// SSH-Konfigurationswerte aus Datenbank laden
builder.Services.AddDbContext<MyDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

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
