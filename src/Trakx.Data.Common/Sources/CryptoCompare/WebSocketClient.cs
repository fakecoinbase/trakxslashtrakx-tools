using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trakx.Data.Common.Sources.CryptoCompare.DTOs;

namespace Trakx.Data.Common.Sources.CryptoCompare
{
    public class WebSocketClient : IAsyncDisposable
    {
        private readonly IApiDetailsProvider _apiDetailsProvider;
        private readonly ClientWebSocket _client;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly ILogger<WebSocketClient> _logger;
        private Task? _listenToWebSocketTask;

        public IWebSocketStreamer WebSocketStreamer { get; }

        public WebSocketClient(IApiDetailsProvider apiDetailsProvider, IWebSocketStreamer webSocketStreamer, ILogger<WebSocketClient> logger)
        {
            _apiDetailsProvider = apiDetailsProvider;
            WebSocketStreamer = webSocketStreamer;
            _logger = logger;
            _client = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Connect()
        {
            _logger.LogInformation("Connecting to CryptoCompare websocket");
            await _client.ConnectAsync(_apiDetailsProvider.WebSocketEndpoint, _cancellationTokenSource.Token).ConfigureAwait(false);
            _logger.LogInformation("CryptoCompare websocket state {0}", State);
            await StartListening(_cancellationTokenSource.Token).ConfigureAwait(false);
        }

        public WebSocketState State => _client.State;

        public async Task AddSubscription(string subscription)
        {
            await AddSubscriptions(new[] {subscription});
        }

        public async Task AddSubscriptions(string[] subscriptions)
        {
            var message = new AddSubscriptionMessage();
            message.Subscriptions.AddRange(subscriptions);
            await _client.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task RemoveSubscription(string subscription)
        {
            await RemoveSubscriptions(new[] { subscription });
        }

        public async Task RemoveSubscriptions(string[] subscriptions)
        {
            var message = new RemoveSubscriptionMessage();
            message.Subscriptions.AddRange(subscriptions);
            await _client.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task StartListening(CancellationToken cancellationToken)
        {
            _listenToWebSocketTask = await Task.Factory.StartNew(async () =>
            {
                while (_client.State == WebSocketState.Open && !_cancellationTokenSource.IsCancellationRequested)
                {
                    var buffer = new ArraySegment<byte>(new byte[4096]);
                    var receiveResult = await _client.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                    var msgBytes = buffer.Skip(buffer.Offset).Take(receiveResult.Count).ToArray();
                    var result = Encoding.UTF8.GetString(msgBytes);

                    if (!string.IsNullOrWhiteSpace(result)) WebSocketStreamer.PublishInboundMessageOnStream(result);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            _logger.LogInformation("Listening to incoming messages");
        }

        private async Task StopListening()
        {
            _cancellationTokenSource.Cancel();
            while (!_listenToWebSocketTask?.IsCanceled ?? false)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }
            _logger.LogInformation("Stopped listening to incoming messages");
        }


        #region IDisposable

        protected virtual async ValueTask DisposeAsync(bool disposing)
        { 
            if (!disposing) return;
            await StopListening();
            _client?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
