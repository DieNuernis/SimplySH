using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplySH.Data;
using SimplySH.Models.SSH;

[Authorize]
public class ConnectionController : Controller
{
    private readonly MyDBContext _context;

    public ConnectionController(MyDBContext context)
    {
        _context = context;
    }

    // Hauptansicht
    public IActionResult Index()
    {
        return View();
    }

    // Verfügbare Verbindungen aus der Datenbank abrufen
    [HttpGet("connections")]
    public IActionResult GetConnections()
    {
        string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var connections = _context.SSHConnections
            .Where(c => c.OwnerId == userId)
            .Select(c => new { name = c.Host, value = c.Host })
            .ToList();

        return Ok(connections);
    }

    // Neue Verbindung zur Datenbank hinzufügen
    [HttpPost("addServer")]
    public async Task<IActionResult> AddServerToDB([FromBody] SSHConnection newServer)
    {
        try
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            newServer.OwnerId = userId;

            _context.SSHConnections.Add(newServer);
            await _context.SaveChangesAsync();

            return Ok(newServer);
        }
        catch (Exception ex)
        {
            return BadRequest($"Fehler beim Speichern in die Datenbank: {ex.Message}");
        }
    }
}