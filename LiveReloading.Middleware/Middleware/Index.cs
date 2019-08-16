namespace LiveReloading.Middleware.Middleware
{
    using Microsoft.AspNetCore.Builder;
    using System;

    public static class PlugLiveReloading
    {
        public  static IApplicationBuilder AddLiveReload(this IApplicationBuilder app, LiveReloadingSettings liveRelaodingSettings = null) {
            
            app.AddWebSocket();
            app.UseMiddleware<LiveReloadingMiddleware>();
            DirectoryWatcher.StatWatching(liveRelaodingSettings ?? new LiveReloadingSettings());
            return app;
        }

        private static void  AddWebSocket(this IApplicationBuilder app)
        {
            var options = new WebSocketOptions {
                KeepAliveInterval = TimeSpan.FromSeconds(240),
                ReceiveBufferSize = 256
            };
            app.UseWebSockets(options);
        }
    }
}