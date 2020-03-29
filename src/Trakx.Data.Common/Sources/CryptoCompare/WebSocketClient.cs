using System;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Data.Common.Sources.CryptoCompare.DTOs;

namespace Trakx.Data.Common.Sources.CryptoCompare
{
    public class WebSocketClient : IDisposable
    {
        private readonly string _apiKey;
        private ClientWebSocket _client;
        private readonly ISubject<string> _incommingMessageSubject;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task<Task> _listenToWebSocketTask;

        public IObservable<string> IncommingMessageStream => _incommingMessageSubject.AsObservable();

        public WebSocketClient(string apiKey)
        {
            _apiKey = apiKey;
            _client = new ClientWebSocket();
            _incommingMessageSubject = new ReplaySubject<string>(1);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Connect()
        {
            await _client.ConnectAsync(new Uri($"wss://streamer.cryptocompare.com/v2?api_key={_apiKey}"), CancellationToken.None);
            StartListening(_cancellationTokenSource.Token);
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

        private void StartListening(CancellationToken cancellationToken)
        {
            _listenToWebSocketTask = Task.Factory.StartNew(async () =>
            {
                while (_client.State == WebSocketState.Open && !_cancellationTokenSource.IsCancellationRequested)
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

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _client?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        #endregion
    }
}
