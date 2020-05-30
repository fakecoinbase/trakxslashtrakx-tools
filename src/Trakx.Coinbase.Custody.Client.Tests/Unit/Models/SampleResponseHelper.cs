using System.IO;
using System.Threading.Tasks;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Models
{
    internal static class SampleResponseHelper
    {
        public static async Task<string> GetSampleResponseContent<T>()
        {
            var sampleResponse = await File.ReadAllTextAsync($"{typeof(T).Name}.json")
                .ConfigureAwait(false);
            
            return sampleResponse;
        }
    }
}
