using System.Linq;
using Flurl.Http.Testing;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit
{
    public static class Util
    {
        public static void ShouldHaveCorrectHeader(this HttpTest test,string apiKey,string passPhrase)
        {
            if(!test.CallLog.Any()) return;
            test.ShouldHaveCalled("https://api.custody.coinbase.com/api/v1/*")
                .WithContentType("application/json")
                .WithHeader(HeaderNames.AccessKey, apiKey)
                .WithHeader(HeaderNames.AccessPassphrase, passPhrase);
        }
    }
}
