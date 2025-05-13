using Microsoft.AspNetCore.Identity;

namespace SimplySH.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string? Vorname { get; set; } // das ist ok
        public string? Nachname { get; set; }
        public string TerminalColor { get; set; } = "#00ff00";
        public int TerminalFontSize { get; set; } = 16;
    }
}
