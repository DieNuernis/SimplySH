using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using SimplySH.Models;
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
                // Holen der Verbindungsdetails aus den Einstellungen
                var connection = _settings.Connections.FirstOrDefault(c => c.Host == connectionName);
                
                if (connection == null)
                {
                    await Clients.Caller.SendAsync("ReceiveOutput", "Verbindung nicht gefunden.\n");
                    return;
                }

                var client = new SshClient(connection.Host, connection.Port, connection.Username, connection.Password);
                client.Connect();

                var shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 4096);
                SshSessionManager.Register(Context.ConnectionId, client, shellStream);

                var connectionId = Context.ConnectionId;

                _ = Task.Run(async () =>
                {
                    var buffer = new byte[4096];
                    while (client.IsConnected && shellStream.CanRead)
                    {
                        int bytesRead = shellStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string rawOutput = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                            // ANSI Escape-Sequenzen entfernen
                            string cleanOutput = AnsiEscapeCodeCleaner.Clean(rawOutput);

                            // Passwort automatisch senden, falls erforderlich
                            if (cleanOutput.ToLower().Contains("assword"))
                            {
                                shellStream.WriteLine(connection.SudoPassword);
                                shellStream.Flush();
                                continue;
                            }
                            await _hubContext.Clients.Client(connectionId)
                                .SendAsync("ReceiveOutput", cleanOutput);
                        }

                        await Task.Delay(20); // Sanftes Throttling
                    }
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveOutput", $"Fehler: {ex.Message}\n");
            }
        }

        // Empfängt einen Befehl vom Client und sendet ihn an den SSH-Stream
        public async Task SendCommand(string command)
        {
            try
            {
                var stream = SshSessionManager.GetShellStream(Context.ConnectionId);
                if (stream == null)
                {
                    await Clients.Caller.SendAsync("ReceiveOutput", "Keine SSH-Verbindung vorhanden.\n");
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


        public async Task Disconnect()
        {
            await SshSessionManager.RemoveSession(Context.ConnectionId);
            await Clients.Caller.SendAsync("ReceiveOutput", "*** SSH-Verbindung getrennt ***\n");
        }
    }
}
