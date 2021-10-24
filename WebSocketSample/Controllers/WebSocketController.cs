using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketSample.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly ILogger<WebSocketController> _logger;
        private static WebSocket webSocket;

        public WebSocketController(ILogger<WebSocketController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Connect")]
        public async Task Index()
        {
            _logger.LogInformation("Accepting websocket connection");
            webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _logger.LogInformation("Websocket connection accepted");

            while (webSocket.State == System.Net.WebSockets.WebSocketState.Open)
            {
                var buffer = new ArraySegment<byte>(new byte[4096]);
                var msgBytes = new List<byte>();
                WebSocketReceiveResult webSocketReceiveRresult;

                do {
                    webSocketReceiveRresult = await webSocket.ReceiveAsync(buffer, default);

                    if (buffer.Array != null && buffer.Array.Length > 0)
                    {
                        for (var i = 0; i < webSocketReceiveRresult.Count; i++)
                        {
                            msgBytes.Add(buffer.Array[i]);
                        }
                    }
                }
                while (!webSocketReceiveRresult.EndOfMessage);

                switch (webSocketReceiveRresult.MessageType)
                {
                    case WebSocketMessageType.Text:
                        var content = Encoding.UTF8.GetString(TrimMessage(msgBytes.ToArray()));
                        _logger.LogDebug("Received Text: {0}", content);
                        break;
                    case WebSocketMessageType.Close:
                        _logger.LogDebug($"{DateTime.Now} Closing websocket...");
                        break;
                }
            }

            _logger.LogWarning($"{DateTime.Now} Websocket has closed : {webSocket.State}");
        }

        [HttpGet("Status")]
        public IActionResult ConnectionStatus()
        {
            return Ok(new
            {
                SocketStaus = webSocket != null ? webSocket.State.ToString() : WebSocketState.None.ToString()
            });
        }

        [HttpGet("Ping")]
        public async Task<IActionResult> Ping()
        {
            var msgBytes = Encoding.UTF8.GetBytes("Are you there");
            await webSocket.SendAsync(new ArraySegment<byte>(msgBytes, 0, msgBytes.Length), 
                WebSocketMessageType.Text, 
                true, 
                CancellationToken.None);
            return Ok();
        }

        private static byte[] TrimMessage(IReadOnlyList<byte> bytes)
        {
            var i = bytes.Count - 1;
            while (i >= 0 && bytes[i] == 0)
            {
                --i;
            }

            return bytes.Take(i + 1).ToArray();
        }
    }
}
