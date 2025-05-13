using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SimplySH.Models.SSH
{
    public class SSHConnection
    {
        [Key]
        public int Id { get; set; }
        public string OwnerId { get; set; }  // ForeignKey zu IdentityUser

        [Required]
        public string Host { get; set; }

        public int Port { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string? SudoPassword { get; set; }
    }
}
