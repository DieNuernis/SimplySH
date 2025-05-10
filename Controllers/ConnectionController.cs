using Microsoft.AspNetCore.Mvc;
using SimplySH.Models;
using System.Text.Json.Nodes;
using System.Text.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SimplySH.Controllers
{
    public class ConnectionController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public ConnectionController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        // Liste aller Verbindungen
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("connections")]
        public IActionResult GetConnections()
        {
            var connections = _config.GetSection("Ssh:Connections").Get<List<SSHConnection>>();
            var result = connections.Select(c => new { name = c.Host, value = c.Host });
            return Ok(result);
        }

        [HttpPost("addServer")]
        public IActionResult AddServer([FromBody] SSHConnection newServer)
        {
            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, "appsettings.json");
                var json = System.IO.File.ReadAllText(filePath);
                var jsonObj = JsonNode.Parse(json);

                var connections = jsonObj?["Ssh"]?["Connections"]?.AsArray() ?? new JsonArray();

                var newConnection = new JsonObject
                {
                    ["Host"] = newServer.Host,
                    ["Port"] = newServer.Port,
                    ["Username"] = newServer.Username,
                    ["Password"] = newServer.Password,
                    ["SudoPassword"] = newServer.SudoPassword ?? ""
                };

                connections.Add(newConnection);

                jsonObj["Ssh"]["Connections"] = connections;

                var newJson = JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, newJson);

                return Ok(newServer);
            }
            catch (Exception ex)
            {
                return BadRequest($"Fehler: {ex.Message}");
            }
        }
    }
}
