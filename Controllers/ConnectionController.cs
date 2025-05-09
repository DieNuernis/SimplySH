using Microsoft.AspNetCore.Mvc;
using SimplySH.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SimplySH.Controllers
{
    public class ConnectionController : Controller
    {
        private readonly IConfiguration _config;

        public ConnectionController(IConfiguration config)
        {
            _config = config;
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
    }
}
