using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Trakx.Data.Common.Sources.CryptoCompare
{
    public class SocketMessage
    {
        public SocketMessage(string action, string format = "streamer")
        {
            Action = action;
            Format = format;
        }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("subs")]
        public List<string> Subscriptions { get; } = new List<string>();

        [JsonPropertyName("format")] 
        public string Format { get; set; }

    }

    public class WebSocketClient
    {
        private readonly string _apiKey;
        private ClientWebSocket _client;
        private readonly ISubject<string> _incommingMessageSubject;

        public IObservable<string> IncommingMessageStream => _incommingMessageSubject.AsObservable();

        public WebSocketClient(string apiKey)
        {
            _apiKey = apiKey;
            _client = new ClientWebSocket();
            _incommingMessageSubject = new ReplaySubject<string>(1);
        }

        public async Task Connect()
        {
            string allStrings = string.Empty;
            await _client.ConnectAsync(new Uri($"wss://streamer.cryptocompare.com/v2?api_key={_apiKey}"), CancellationToken.None);
        }

        public WebSocketState State => _client.State;

        public async Task AddSubscription(CancellationToken cancellationToken)
        {
            var message = new SocketMessage("SubAdd");
            message.Subscriptions.Add("5~CCCAGG~BTC~USD");
            await _client.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)),
                WebSocketMessageType.Text, true, CancellationToken.None);

            var listenToWebSocketTask = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var buffer = new ArraySegment<byte>(new byte[4096]);
                    var receiveResult = await _client.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                    var msgBytes = buffer.Skip(buffer.Offset).Take(receiveResult.Count).ToArray();
                    var result = Encoding.UTF8.GetString(msgBytes);

                    if (!string.IsNullOrWhiteSpace(result))
                        _incommingMessageSubject.OnNext(result);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
