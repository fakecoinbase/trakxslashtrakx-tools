using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Data.Common.Sources.CryptoCompare;
using Trakx.Data.Tests.Tools;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Data.Tests.Integration.Common.Sources.CryptoCompare
{
    public class WebSocketClientTests
    {
        private readonly ITestOutputHelper _output;
        private readonly WebSocketClient _client;

        public WebSocketClientTests(ITestOutputHelper output)
        {
            _output = output;
            _client = new WebSocketClient(Secrets.CryptoCompareApiKey);
        }

        [Fact(Skip = "needs a key")]
        public async Task WebSocketClient_should_receive_updates()
        {
            await _client.Connect();
            _client.State.Should().Be(WebSocketState.Open);
             
            using var cancellationTokenSource = new CancellationTokenSource();

            await _client.AddSubscription(cancellationTokenSource.Token).ConfigureAwait(false);
            var message = await _client.IncommingMessageStream.FirstAsync();
            _output.WriteLine(message);

            message.Should().NotBeNullOrWhiteSpace();
            message.Should().Contain("PRICE");

            cancellationTokenSource.Cancel();
        }
    }
}
