using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit
{
    public class CoinbaseClientTests
    {
        private readonly CoinbaseClient _client;
        private readonly ICurrencyEndpoint _currencyEndpoint;

        public CoinbaseClientTests(ITestOutputHelper output)
        {
            _currencyEndpoint = Substitute.For<ICurrencyEndpoint>();
            _client = new CoinbaseClient(null, null, null, _currencyEndpoint);
        }

        private PagedResponse<Currency> GetPagedResponse(int pageNumber, int size)
        {
            var data = Enumerable.Range(0, size).Select(i =>
                new Currency
                {
                    Decimals = (ushort) (i % size),
                    Symbol = $"SYM{pageNumber}N{i}",
                    Name = $"Currency {pageNumber} {i}"
                }).ToArray();
            var pageResponse = new PagedResponse<Currency>
            {
                Data = data,
                Pagination = new Pagination
                {
                    Before = $"SYM{pageNumber - 1}N{size}",
                    After = pageNumber == 0 ? null : $"SYM{pageNumber - 1}N{size}"
                }
            };
            return pageResponse;
        }

        [Fact]
        public async Task GetCurrencies_should_enumerate_page_responses_lazily()
        {
            var apiCallNumber = 0;

            _currencyEndpoint.ListCurrenciesAsync().ReturnsForAnyArgs(ci =>
            {
                var page = GetPagedResponse(apiCallNumber, 10);
                apiCallNumber++;
                return page;
            });

            await _client.GetCurrencies(new PaginationOptions(pageSize: 10)).Take(35).ToListAsync();

            await _currencyEndpoint.ReceivedWithAnyArgs(4).ListCurrenciesAsync();
        }

        [Fact]
        public async Task GetCurrencies_should_enumerate_page_until_the_end()
        {
            _currencyEndpoint.ListCurrenciesAsync().ReturnsForAnyArgs(
                GetPagedResponse(0, 10),
                GetPagedResponse(1, 5));

            var allCurrencies = await _client.GetCurrencies(new PaginationOptions(pageSize: 10)).ToListAsync();

            await _currencyEndpoint.ReceivedWithAnyArgs(2).ListCurrenciesAsync();
            allCurrencies.Count.Should().Be(15);
        }
    }
}
