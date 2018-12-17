using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketData.Feeds.Tests.Utils
{
    public class ThrottledClient
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private readonly ITestOutputHelper _output;
        private readonly int _millisecondsDelay;

        public ThrottledClient(ITestOutputHelper output, int millisecondsDelay = 100)
        {
            _output = output;
            _millisecondsDelay = millisecondsDelay;
        }

        public async Task<string> SendAsync(string message)
        {
            await Semaphore.WaitAsync();
            try
            {
                
                _output.WriteLine($"[{DateTime.Now:HHmmssfff}] - processing {message}");
                await Task.Delay(10);
                return $" - processing {message} done at {DateTime.Now:HHmmssfff}";
            }
            finally
            {
                await Task.Delay(_millisecondsDelay);
                Semaphore.Release(1);
            }
        }
    }

    public class ThrottleTests
    {
        private readonly ITestOutputHelper _output;

        public ThrottleTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "Just a draft")]
        public async Task Test()
        {
            var throttledClient = new ThrottledClient(_output, 100);

            var tasks = Enumerable.Range(0, 10).ToList().Select(
                async i =>
                    {
                        _output.WriteLine($"[{DateTime.Now:HHmmssfff}] - calling {i}");
                        var result = await throttledClient.SendAsync(i.ToString());
                        _output.WriteLine($"[{DateTime.Now:HHmmssfff}] - result is {result}");
                    });

            await Task.WhenAll(tasks);
        }
    }
}
