namespace LiveReloading.Middleware.Middleware
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Text;
    using static LiveReloading.Middleware.Shared.FunctionalExtensions;


    public class ExtendedResponseStream : Stream
    {
        private readonly Stream _stream;
        private readonly HttpContext _httpContext;

        public ExtendedResponseStream(Stream stream, HttpContext httpContext)
        {
            _stream = stream;
            _httpContext = httpContext;
            CanWrite = true;
        }

        public override bool CanRead { get; }

        public override bool CanSeek { get; }

        public override bool CanWrite { get; }

        public override long Length { get; }

        public override long Position { set; get; }

        public override void Flush() => _stream.Flush();


        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);


        public override void SetLength(long value)
        {
            _stream.SetLength(value);
            IsHtmlContents();
        }

        private bool IsHtmlContents()
        {
            var response = _httpContext.Response;
            string contentType = response.ContentType;
            if (contentType == null) return false;
            var result = response.StatusCode == StatusCodes.Status200OK &&
            (contentType.Contains("text/html", StringComparison.InvariantCultureIgnoreCase) ||
            contentType.Contains("utf-8", StringComparison.InvariantCultureIgnoreCase) ||
            contentType.Contains("charset=", StringComparison.InvariantCultureIgnoreCase));

            if (result)
                _httpContext.Response.ContentType = null;
            return result;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!IsHtmlContents()) _stream.Write(buffer, offset, count);
            else AddLiveRealoadScript(buffer, offset, count);
        }

        private void AddLiveRealoadScript(byte[] buffer, int offset, int count)
        {
            string scriptBlock = GetScriptBlock();
            var rv = Combined(buffer, Encoding.UTF8.GetBytes(scriptBlock));
            _stream.Write(rv, offset, rv.Length);

        }

        private string GetScriptBlock()
        {
            var portocol = $"ws{(_httpContext.Request.IsHttps ? "s" : string.Empty)}";
            var host = _httpContext.Request.Host;
            var url = $"{portocol}://{host.Host}:{host.Port}/";
            return @"<script>
                function InitLiveReload() {
                var socket = createWebSocket();

                socket.onopen = socketOpened;
                socket.onmessage = messageReceived;
                socket.onclose = socketClosed;
                socket.onerror = handleError;


                function handleError(event) {
                    console.log('error occured : ', JSON.stringify(event));
                }

                function messageReceived(event) {
                    if (event.data === 'refresh') location.reload(true);
                }

                function socketOpened(event) {
                    console.log('connection opened : ', JSON.stringify(event));
                }

                function socketClosed(event) {
                    console.log('connection closed : ', JSON.stringify(event));
                }

                function createWebSocket() {
                    var result = new WebSocket('"+ url + @"');
                    return result;
                }
            }

        InitLiveReload();

            </script>";
        }
    }
}
