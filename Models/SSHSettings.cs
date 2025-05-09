namespace SimplySH.Models
{
    public class SSHConnection
    {
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
