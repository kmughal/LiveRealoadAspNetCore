namespace LiveReloading.Middleware.Middleware
{
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    public interface IWebSocketsStore
    {
        void AddIfNotPresent(WebSocket socket);
       
        void RemoveIfPresent(WebSocket socket);
    }
}