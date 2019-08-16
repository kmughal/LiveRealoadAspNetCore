using LiveReloading.Middleware.Shared;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace LiveReloading.Middleware.Middleware
{
    public class LiveReloadingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebSocketsStore _webSocketsStore = new InMemoryWebSocketsStore(); // This should be injected via constructor!

        public LiveReloadingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!IsWebSocketRequest(context))
            {
                await AppendLiveReloadingScriptToResponse(context);
                return;
            }

            using (var socket = await GetActiveWebSocketFromContext(context))
            {
                _webSocketsStore.AddIfNotPresent(socket);
                await SocketWait(socket);
            }
        }

        private async Task AppendLiveReloadingScriptToResponse(HttpContext context)
        {
            using (var extendedResponse = new ExtendedResponseStream(context.Response.Body,context))
            {
                context.Response.Body = extendedResponse;
                await _next(context);
            }
            
        }

        private async Task SocketWait(WebSocket socket)
        {
            var buffer = new byte[1024];
            while (socket.State.HasFlag(WebSocketState.Open))
            {
                try
                {
                    await socket.ReceiveAsync(buffer, CancellationToken.None);
                }
                catch
                {
                    break;
                }
            }
            _webSocketsStore.RemoveIfPresent(socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "*** Socket Closed ****", CancellationToken.None);
        }

        private async Task<WebSocket> GetActiveWebSocketFromContext(HttpContext context) => await context.WebSockets.AcceptWebSocketAsync();

        private bool IsWebSocketRequest(HttpContext context) => context.WebSockets.IsWebSocketRequest;

    }
}