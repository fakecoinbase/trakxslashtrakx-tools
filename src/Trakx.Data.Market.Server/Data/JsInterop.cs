using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Trakx.Data.Market.Server.Data
{
    public class JsInterop
    {
        private readonly IJSRuntime _jsRuntime;

        public JsInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> Greet(string name)
        {
            var greeting = await _jsRuntime.InvokeAsync<string>("Greeter", name).ConfigureAwait(false);
            return greeting;
        }
    }
}
