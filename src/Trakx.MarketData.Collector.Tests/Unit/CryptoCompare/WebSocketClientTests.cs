﻿using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Trakx.Common.Interfaces;
using Trakx.MarketData.Collector.CryptoCompare;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Outbound;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketData.Collector.Tests.Unit.CryptoCompare
{
    public class WebSocketClientTests
    {
        private readonly IApiDetailsProvider _apiDetailsProvider;
        private readonly IClientWebsocket _innerClient;
        private readonly IWebSocketStreamer _webSocketStreamer;
        private readonly CryptoCompareWebSocketClient _webSocketClient;

        public WebSocketClientTests(ITestOutputHelper output)
        {
            _apiDetailsProvider = Substitute.For<IApiDetailsProvider>();
            _innerClient = Substitute.For<IClientWebsocket>();
            _webSocketStreamer = Substitute.For<IWebSocketStreamer>();
            var logger = output.ToLogger<CryptoCompareWebSocketClient>();

            _webSocketClient = new CryptoCompareWebSocketClient(_innerClient, _apiDetailsProvider, _webSocketStreamer, logger);
        }

        [Fact]
        public async Task Connect_should_take_URI_from_apiDetailsProvider()
        {
            var expectedUri = new Uri("wss://hello/apiKey");
            _apiDetailsProvider.WebSocketEndpoint.Returns(expectedUri);
            await _webSocketClient.Connect();

            await _innerClient.Received(1).ConnectAsync(expectedUri, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Connect_should_start_listening_task()
        {
            _innerClient.State.Returns(WebSocketState.Open);
            await _webSocketClient.Connect();
            await _innerClient.Received(1).ReceiveAsync(Arg.Any<ArraySegment<byte>>(), Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData(WebSocketState.None)]
        [InlineData(WebSocketState.Aborted)]
        [InlineData(WebSocketState.CloseSent)]
        [InlineData(WebSocketState.CloseReceived)]
        [InlineData(WebSocketState.Closed)]
        [InlineData(WebSocketState.Connecting)]
        public async Task Connect_should_not_start_listening_loop_task_when_websocket_not_open(WebSocketState state)
        {
            _innerClient.State.Returns(state);
            await _webSocketClient.Connect();
            ListeningLoopShouldHaveEnded();
            await _innerClient.DidNotReceiveWithAnyArgs().ReceiveAsync(default, default);
        }

        private void ListeningLoopShouldHaveEnded()
        {
            ((int?) _webSocketClient.ListenInboundMessagesTaskStatus ?? 0).Should()
                .BeGreaterOrEqualTo((int) TaskStatus.RanToCompletion);
        }

        [Fact]
        public async Task StartListening_should_forward_UTF8_content_to_WebSocketStreamer()
        {
            _innerClient.State.Returns(WebSocketState.Open, WebSocketState.Open, WebSocketState.Open, WebSocketState.Closed);
            var rawMessage = "message";
            SetupFakeMessageReception(rawMessage);
            await _webSocketClient.Connect();
            while (!_webSocketClient.WebSocketStreamer.ReceivedCalls().Any())
            {
                await Task.Delay(10);
            }
            _webSocketStreamer.Received(1).PublishInboundMessageOnStream(rawMessage);
        }

        [Fact]
        public async Task StartListening_should_not_forward_empty_UTF8_content_to_WebSocketStreamer()
        {
            _innerClient.State.Returns(WebSocketState.Open, WebSocketState.Open, WebSocketState.Closed);
            SetupFakeMessageReception("");
            await _webSocketClient.Connect();

            _webSocketStreamer.DidNotReceiveWithAnyArgs().PublishInboundMessageOnStream(default);
        }

        [Fact]
        public async Task Dispose_should_end_listening_task_and_dispose_inner_client()
        {
            _innerClient.State.Returns(WebSocketState.Open);
            SetupFakeMessageReception("hello");
            SetupFakeCloseSideEffect();

            await _webSocketClient.Connect();

            while (!_webSocketClient.WebSocketStreamer.ReceivedCalls().Any())
            {
                await Task.Delay(10);
            }

            await _webSocketClient.DisposeAsync();

            ListeningLoopShouldHaveEnded();
            await _innerClient.Received(1).CloseAsync(Arg.Is(WebSocketCloseStatus.NormalClosure), 
                Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        private void SetupFakeMessageReception(string rawMessage, bool isCloseMessage = false)
        {
            var messageBytes = Encoding.UTF8.GetBytes(rawMessage).AsMemory();
            _innerClient.ReceiveAsync(Arg.Any<ArraySegment<byte>>(), Arg.Any<CancellationToken>())
                .Returns(async ci =>
                {
                    ((CancellationToken)ci[1]).ThrowIfCancellationRequested();
                    await Task.Delay(100).ConfigureAwait(false);
                    var webSocketMessageType = isCloseMessage ? WebSocketMessageType.Close : WebSocketMessageType.Text;
                    return new WebSocketReceiveResult(messageBytes.Length, webSocketMessageType, true);
                })
                .AndDoes(ci =>
                {
                    var buffer = (ArraySegment<byte>) ci[0];
                    messageBytes.TryCopyTo(buffer);
                });
        }

        private void SetupFakeCloseSideEffect()
        {
            _innerClient.CloseAsync(Arg.Any<WebSocketCloseStatus>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask)
                .AndDoes(ci => SetupFakeMessageReception("bye", true));
        }

        [Fact]
        public async Task AddSubscription_should_send_correct_outbound_message()
        {
            var subscription = new AggregateIndexSubscription("ETH", "usd");
            await _webSocketClient.AddSubscriptions(subscription);

            await _innerClient.Received(1).SendAsync(Arg.Any<ArraySegment<byte>>(), WebSocketMessageType.Text, true, Arg.Any<CancellationToken>());
            var bytes = (ArraySegment<byte>) _innerClient.ReceivedCalls()
                .Single(c => c.GetMethodInfo().Name == nameof(_innerClient.SendAsync)).GetArguments().First();
            var utf8 = Encoding.UTF8.GetString(bytes);
            utf8.Should().Contain(AddSubscriptionMessage.SubAdd);
            utf8.Should().Contain(subscription.ToString());
        }

        [Fact]
        public async Task RemoveSubscription_should_send_correct_outbound_message()
        {
            var subscription = new TickerSubscription("Binance", "eth", "usd");
            await _webSocketClient.RemoveSubscriptions(subscription);

            await _innerClient.Received(1).SendAsync(Arg.Any<ArraySegment<byte>>(), WebSocketMessageType.Text, true, Arg.Any<CancellationToken>());
            var bytes = (ArraySegment<byte>)_innerClient.ReceivedCalls() 
                .Single(c => c.GetMethodInfo().Name == nameof(_innerClient.SendAsync)).GetArguments().First();
            var utf8 = Encoding.UTF8.GetString(bytes);
            utf8.Should().Contain(RemoveSubscriptionMessage.SubRemove);
            utf8.Should().Contain(subscription.ToString());
        }
    }
}
