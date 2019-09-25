using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketData.Feeds.Tests.ApiClients
{
    public class MessariTests
    {
        private readonly ITestOutputHelper _output;

        public MessariTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetTop20Erc20()
        {
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            handler.UseDefaultCredentials = true;
            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(@"https://data.messari.io/");

            var uriBuilder = new UriBuilder(client.BaseAddress);
            uriBuilder.Path = "api/v1/assets";
            //var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
            //paramValues.Add("with-metrics", "");
            //paramValues.Add("with-profiles", "");
            //uriBuilder.Query = paramValues.ToString();
            uriBuilder.Query = "?with-metrics&with-profiles";


            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var response = await client.SendAsync(request);
            var payload = await response.Content.ReadAsStringAsync();
            _output.WriteLine(payload);

            var queryable = JObject.Parse(payload);
            var data = queryable["data"];

            var mkr = data.Where(d => d["symbol"].ToString() == "mkr");

            var erc20s = data.Where(d => d["profile"]["token_details"]["type"].ToString().Contains("ERC")).ToList();
        }
    }
}

