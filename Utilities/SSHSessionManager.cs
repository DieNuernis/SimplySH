using Renci.SshNet;
using System.Collections.Concurrent;

namespace SimplySH.Utilities
{
    public static class SshSessionManager
    {
        private static ConcurrentDictionary<string, SshClient> _clients = new();
        private static ConcurrentDictionary<string, ShellStream> _streams = new();

        // Neue SSH-Client-/Stream-Paare speichern
        public static void Register(string connectionId, SshClient client, ShellStream stream)
        {
            _clients[connectionId] = client;
            _streams[connectionId] = stream;
        }

        // Stream des angegebenen ConnectionId abrufen
        public static ShellStream GetShellStream(string connectionId)
        {
            _streams.TryGetValue(connectionId, out var stream);
            return stream;
        }

        // SSH-Client des angegebenen ConnectionId abrufen
        public static SshClient GetSshClient(string connectionId)
        {
            _clients.TryGetValue(connectionId, out var client);
            return client;
        }
    }
}
