using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplySH.Hubs;
using SimplySH.Models;

var builder = WebApplication.CreateBuilder(args);

// SSH-Konfigurationswerte aus appsettings.json laden
builder.Services.Configure<SSHSettings>(builder.Configuration.GetSection("Ssh"));

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
