using System.IO;
using System.Threading.Tasks;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Models
{
    internal static class SampleResponseHelper
    {
        public static async Task<string> GetSampleResponseContent(string fileName)
        {
            var sampleResponse = await File.ReadAllTextAsync($"{fileName}.json")
                .ConfigureAwait(false);
            
            return sampleResponse;
        }
    }
}
