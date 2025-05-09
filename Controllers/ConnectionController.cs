using Microsoft.AspNetCore.Mvc;

namespace SimplySH.Controllers
{
    public class ConnectionController : Controller
    {
        // Liste aller Verbindungen
        public IActionResult Index()
        {
            return View();
        }
    }
}
