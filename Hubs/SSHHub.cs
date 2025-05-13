using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using SimplySH.Models.SSH;
using SimplySH.Utilities;
using System.Text;

namespace SimplySH.Hubs
{
    public class SSHHub : Hub
    {
        private readonly IHubContext<SSHHub> _hubContext;
        private readonly SSHSettings _settings;

        public SSHHub(IOptions<SSHSettings> sshOptions, IHubContext<SSHHub> hubContext)
        {
            _settings = sshOptions.Value;
            _hubContext = hubContext;
        }

        public async Task Connect(string connectionName)
        {
            try
            {
                var connection = _settings.Connections.FirstOrDefault(c => c.Host == connectionName);

                if (connection == null)
                {
                    await Clients.Caller.SendAsync("ReceiveOutput", "*** Verbindung nicht gefunden. ***\n");
                    return;
                }

                var client = new SshClient(connection.Host, connection.Port, connection.Username, connection.Password);
                client.Connect();

                var shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 4096);
                SshSessionManager.Register(Context.ConnectionId, client, shellStream);

                string connectionId = Context.ConnectionId;

                _ = Task.Run(async () =>
                {
                    var buffer = new byte[4096];
                    string lastOutput = string.Empty;

                    while (client.IsConnected && shellStream.CanRead)
                    {
                        int bytesRead = shellStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string rawOutput = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            string cleanOutput = AnsiEscapeCodeCleaner.Clean(rawOutput);

                            if (!string.Equals(cleanOutput, lastOutput))
                            {
                                lastOutput = cleanOutput;
                                await _hubContext.Clients.Client(connectionId)
                                    .SendAsync("ReceiveOutput", cleanOutput);
                            }

                            if (cleanOutput.ToLower().Contains("assword"))
                            {
                                shellStream.WriteLine(connection.SudoPassword);
                                shellStream.Flush();
                            }
                        }

                        await Task.Delay(20);
                    }
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveOutput", $"Fehler: {ex.Message}\n");
            }
        }

        public async Task SendCommand(string command)
        {
            try
            {
                var stream = SshSessionManager.GetShellStream(Context.ConnectionId);
                if (stream == null)
                {
                    await Clients.Caller.SendAsync("ReceiveOutput", "*** Keine SSH-Verbindung vorhanden. ***\n");
                    return;
                }

                stream.WriteLine(command);
                stream.Flush();
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveOutput", $"Fehler: {ex.Message}\n");
            }
        }
    }
}
