using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trakx.Data.Common.Sources.CryptoCompare;
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
            _client = new WebSocketClient("");
        }

        [Fact]
        public async Task WebSocketClient_should_receive_updates()
        {
            var result = await _client.Connect();
            _output.WriteLine(result);
        }
    }
}
