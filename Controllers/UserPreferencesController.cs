using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimplySH.Models.Auth;

namespace SimplySH.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserPreferencesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserPreferencesController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetPreferences()
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(new
            {
                color = user.TerminalColor,
                fontSize = user.TerminalFontSize
            });
        }

        [HttpPost]
        public async Task<IActionResult> SavePreferences([FromBody] PreferencesDto prefs)
        {
            var user = await _userManager.GetUserAsync(User);

            user.TerminalColor = prefs.Color;
            user.TerminalFontSize = prefs.FontSize;

            await _userManager.UpdateAsync(user);

            return Ok();
        }

        public class PreferencesDto
        {
            public string Color { get; set; }
            public int FontSize { get; set; }
        }
    }
}