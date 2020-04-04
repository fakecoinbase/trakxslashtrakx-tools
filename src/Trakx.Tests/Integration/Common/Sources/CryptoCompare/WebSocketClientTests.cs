﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Data.Common.Interfaces;
using Trakx.Data.Common.Sources.CryptoCompare;
using Trakx.Data.Common.Sources.CryptoCompare.DTOs.Inbound;
using Trakx.Data.Common.Sources.CryptoCompare.DTOs.Outbound;
using Trakx.Data.Tests.Tools;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Data.Tests.Integration.Common.Sources.CryptoCompare
{
    //AsyncDispose not yet supported by XUnit so let's implement both...
    public sealed class WebSocketClientTests : IAsyncDisposable, IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly WebSocketClient _client;

        public WebSocketClientTests(ITestOutputHelper output)
        {
            _output = output;
            var logger = output.ToLogger<WebSocketClient>();
            var streamer = new WebSocketStreamer(output.ToLogger<WebSocketStreamer>());
            var apiDetailsProvider = new ApiDetailsProvider(Secrets.CryptoCompareApiKey);
            var clientWebSocket = new WrappedClientWebsocket();
            _client = new WebSocketClient(clientWebSocket, apiDetailsProvider, streamer, logger);
        }

        [Fact]
        public async Task WebSocketClient_should_receive_Trade_updates()
        {
            var btcUsdSubscription = new TradeSubscription("Coinbase", "btc", "usd");
            await RunTestForSubscriptionType<Trade>(btcUsdSubscription);
        }

        [Fact]
        public async Task WebSocketClient_should_receive_Ticker_updates()
        {
            var btcUsdSubscription = new TickerSubscription("Bitfinex", "eth", "usd");
            await RunTestForSubscriptionType<Ticker>(btcUsdSubscription);
        }

        [Fact]
        public async Task WebSocketClient_should_receive_AggregateIndex_updates()
        {
            var btcUsdSubscription = new AggregateIndexSubscription("btc", "usd");
            await RunTestForSubscriptionType<AggregateIndex>(btcUsdSubscription);
        }

        [Fact]
        public async Task WebSocketClient_should_receive_Ohlc_updates()
        {
            var btcUsdSubscription = new OhlcSubscription("CCCAGG", "eth", "usd", TimeSpan.FromHours(1));
            await RunTestForSubscriptionType<Ohlc>(btcUsdSubscription);
        }

        private async Task RunTestForSubscriptionType<T>(ICryptoCompareSubscription subscription)
        {
            await _client.Connect();
            _client.State.Should().Be(WebSocketState.Open);

            var messagesReceived = new List<InboundMessageBase>();

            using var inboundMessageStream = _client.WebSocketStreamer.AllInboundMessagesStream
                .SubscribeOn(Scheduler.Default)
                .Take(50)
                .Subscribe(m =>
                {
                    _output.WriteLine(JsonSerializer.Serialize(m));
                    messagesReceived.Add(m);
                });


            await _client.AddSubscriptions(subscription).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            messagesReceived.Count.Should().BeGreaterOrEqualTo(3);

            messagesReceived.OfType<T>().Count().Should().BeGreaterOrEqualTo(1);
            messagesReceived.OfType<SubscribeComplete>().Count().Should().BeGreaterOrEqualTo(1);
            messagesReceived.OfType<LoadComplete>().Count().Should().BeGreaterOrEqualTo(1);


            await _client.RemoveSubscriptions(subscription).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            messagesReceived.Count.Should().BeGreaterOrEqualTo(5);
            messagesReceived.OfType<SubscribeComplete>().Count().Should().BeGreaterOrEqualTo(1);
            messagesReceived.OfType<LoadComplete>().Count().Should().BeGreaterOrEqualTo(1);

            await _client.DisposeAsync();
            _client.State.Should().Be(WebSocketState.Closed);
        }

        #region IDisposable

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            //await _client.DisposeAsync();
        }

        #endregion

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        #endregion
    }
}
