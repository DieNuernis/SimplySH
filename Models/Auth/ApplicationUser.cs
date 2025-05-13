using Microsoft.AspNetCore.Identity;

namespace SimplySH.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string? Vorname { get; set; } // das ist ok
        public string? Nachname { get; set; }
    }
}
