using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Data.Common.Sources.CryptoCompare;
using Trakx.Data.Common.Sources.CryptoCompare.DTOs;
using Trakx.Data.Tests.Tools;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Data.Tests.Integration.Common.Sources.CryptoCompare
{
    public sealed class WebSocketClientTests : IAsyncDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly WebSocketClient _client;

        public WebSocketClientTests(ITestOutputHelper output)
        {
            _output = output;
            var logger = output.ToLogger<WebSocketClient>();
            var streamer = new WebSocketStreamer(output.ToLogger<WebSocketStreamer>());
            _client = new WebSocketClient(Secrets.CryptoCompareApiKey, streamer, logger);
        }

        //[Fact(Skip = "needs a key")]
        [Fact]
        public async Task WebSocketClient_should_receive_updates()
        {
            await _client.Connect();
            _client.State.Should().Be(WebSocketState.Open);

            var messagesReceived = new List<WebSocketInboundMessage>();

            using var subscription = _client.WebSocketStreamer.AllInboundMessagesStream
                .SubscribeOn(Scheduler.Default)
                .Take(10)
                .Subscribe(m =>
                {
                    _output.WriteLine(JsonSerializer.Serialize(m));
                    messagesReceived.Add(m);
                });

            var btcUsdSubscription = "5~CCCAGG~BTC~USD";
            await _client.AddSubscription(btcUsdSubscription).ConfigureAwait(false);

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            messagesReceived.Count.Should().BeGreaterOrEqualTo(3);

            messagesReceived.OfType<AggregateIndexResponse>().Count().Should().BeGreaterOrEqualTo(1);
            messagesReceived.OfType<SubscribeCompleteMessage>().Count().Should().BeGreaterOrEqualTo(1);
            messagesReceived.OfType<LoadCompleteMessage>().Count().Should().BeGreaterOrEqualTo(1);


            await _client.RemoveSubscription(btcUsdSubscription).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            messagesReceived.Count.Should().BeGreaterOrEqualTo(5);
            messagesReceived.OfType<SubscribeCompleteMessage>().Count().Should().BeGreaterOrEqualTo(1);
            messagesReceived.OfType<LoadCompleteMessage>().Count().Should().BeGreaterOrEqualTo(1);
        }

        #region IDisposable

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await _client.DisposeAsync();
        }

        #endregion
    }
}
