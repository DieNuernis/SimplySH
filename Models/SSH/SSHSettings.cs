using Microsoft.EntityFrameworkCore;

namespace SimplySH.Models.SSH
{
    public class SSHConnection
    {
        public int Id { get; set; } // Unique identifier for the connection
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SudoPassword { get; set; }
    }

    public class SSHSettings
    {
        public List<SSHConnection> Connections { get; set; }
    }
}
