namespace LiveReloading.Middleware.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class InMemoryWebSocketsStore : IWebSocketsStore
    {
        private static readonly HashSet<WebSocket> _webSockets = new HashSet<WebSocket>();

        public static async Task NotifyChange()
        {
            var refreshMessage = GetRefreshMessage();
            foreach (var socket in _webSockets)
            {
                await socket.SendAsync(refreshMessage, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public void AddIfNotPresent(WebSocket socket)
        {
            if (ContainsSocket(socket)) return;
            _webSockets.Add(socket);
        }

        public void RemoveIfPresent(WebSocket socket)
        {
            if (!ContainsSocket(socket)) return;
            _webSockets.Remove(socket);
        }

        private bool ContainsSocket(WebSocket socket) => _webSockets.Contains(socket);

        private static ArraySegment<byte> GetRefreshMessage() =>
          new ArraySegment<byte>(Encoding.UTF8.GetBytes("refresh"));
    }
}
