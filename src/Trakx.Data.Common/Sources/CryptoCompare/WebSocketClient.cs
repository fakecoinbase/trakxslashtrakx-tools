using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Trakx.Data.Common.Sources.CryptoCompare
{
    public class SocketMessage
    {
        public SocketMessage(string action)
        {
            
        }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("subs")]
        public List<string> Subscriptions { get; } = new List<string>();

    }

    public class WebSocketClient
    {
        private readonly string _apiKey;
        private ClientWebSocket _client;

        public WebSocketClient(string apiKey)
        {
            _apiKey = apiKey;
            _client = new ClientWebSocket();
        }

        public async Task<string> Connect()
        {
            string allStrings = string.Empty;
            await _client.ConnectAsync(new Uri($"wss://streamer.cryptocompare.com/v2?api_key={_apiKey}"), CancellationToken.None);
            var message = new SocketMessage("SubAdd");
            message.Subscriptions.Add("5~CCCAGG~BTC~USD");
            await _client.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)),
                WebSocketMessageType.Text, true, CancellationToken.None);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var buffer = new Memory<byte>();
                var received = await _client.ReceiveAsync(buffer, cancellationTokenSource.Token);
                var result = Encoding.UTF8.GetString(buffer.ToArray());
                allStrings += result;
            }

            return allStrings;
        }
    }
}
