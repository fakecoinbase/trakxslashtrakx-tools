using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trakx.Data.Common.Interfaces;
using Trakx.Data.Common.Sources.CryptoCompare.DTOs.Outbound;

namespace Trakx.Data.Common.Sources.CryptoCompare
{
    public class WebSocketClient : IAsyncDisposable, ICryptoCompareWebSocketClient, IDisposable
    {
        private readonly IApiDetailsProvider _apiDetailsProvider;
        private readonly IClientWebsocket _client;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly ILogger<WebSocketClient> _logger;
        private Task? _listenToWebSocketTask;

        public IWebSocketStreamer WebSocketStreamer { get; }

        public WebSocketClient(IClientWebsocket clientWebSocket, 
            IApiDetailsProvider apiDetailsProvider, 
            IWebSocketStreamer webSocketStreamer, 
            ILogger<WebSocketClient> logger)
        {
            _apiDetailsProvider = apiDetailsProvider;
            _client = clientWebSocket;
            WebSocketStreamer = webSocketStreamer;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Connect()
        {
            _logger.LogInformation("Opening CryptoCompare websocket");
            if (_client.State != WebSocketState.Open) 
                await _client.ConnectAsync(_apiDetailsProvider.WebSocketEndpoint, _cancellationTokenSource.Token).ConfigureAwait(false);
            _logger.LogInformation("CryptoCompare websocket state {0}", State);
            await StartListening(_cancellationTokenSource.Token).ConfigureAwait(false);
        }

        public WebSocketState State => _client.State;
        public TaskStatus? ListenInboundMessagesTaskStatus => _listenToWebSocketTask?.Status;

        public async Task AddSubscriptions(params ICryptoCompareSubscription[] subscriptions)
        {
            var message = new AddSubscriptionMessage(subscriptions);
            await _client.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task RemoveSubscriptions(params ICryptoCompareSubscription[] subscriptions)
        {
            var message = new RemoveSubscriptionMessage(subscriptions);
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
                    if (receiveResult.MessageType == WebSocketMessageType.Close) break;
                    var msgBytes = buffer.Skip(buffer.Offset).Take(receiveResult.Count).ToArray();
                    var result = Encoding.UTF8.GetString(msgBytes);

                    if (!string.IsNullOrWhiteSpace(result)) WebSocketStreamer.PublishInboundMessageOnStream(result);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
            _logger.LogInformation("Listening to incoming messages");   
        }

        public async Task Disconnect()
        {
            _logger.LogInformation("Closing CryptoCompare websocket");
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure,
                "CryptoCompare WebClient getting disposed.",
                _cancellationTokenSource.Token);
            _logger.LogInformation("Closing CryptoCompare websocket");
        }

        private async Task StopListening()
        {
            while (_listenToWebSocketTask != null && _listenToWebSocketTask.Status < TaskStatus.RanToCompletion)
            {
                await Task.Delay(100, _cancellationTokenSource.Token).ConfigureAwait(false);
            }
            _logger.LogInformation("CryptoCompare websocket state {0}", State);
        }

        #region IDisposable

        protected virtual async ValueTask DisposeAsync(bool disposing)
        { 
            if (!disposing) return;
            _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
            await Disconnect().ConfigureAwait(false);
            await StopListening().ConfigureAwait(false);

            _cancellationTokenSource?.Dispose();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            DisposeAsync(disposing).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
