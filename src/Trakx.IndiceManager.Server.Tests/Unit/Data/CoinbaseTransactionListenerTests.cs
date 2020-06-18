using System;
using System.Reactive.Concurrency;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Interfaces;
using Trakx.IndiceManager.Server.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Server.Tests.Unit.Data
{
    public sealed class CoinbaseTransactionListenerTests : IDisposable
    {
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly CoinbaseTransactionListener _coinbaseListener;
        private readonly TestScheduler _testScheduler;
        private readonly TimeSpan _runningTime;
        private readonly IMemoryCache _cache;
        private readonly ITransactionDataProvider _transactionDataProvider;

        public CoinbaseTransactionListenerTests(ITestOutputHelper output)
        {
            _cache = Substitute.For<IMemoryCache>();
            _transactionDataProvider = Substitute.For<ITransactionDataProvider>();
            var serviceScopeFactory = PrepareScopeResolution();
            _coinbaseClient = Substitute.For<ICoinbaseClient>();
            _testScheduler = new TestScheduler();
            _runningTime = CoinbaseTransactionListener
                .PollingInterval.Multiply(10)
                .Add(TimeSpan.FromMilliseconds(100));
            _coinbaseListener = new CoinbaseTransactionListener(
                _coinbaseClient,
                output.ToLogger<CoinbaseTransactionListener>(), serviceScopeFactory,_cache, _testScheduler);
        }

        private IServiceScopeFactory PrepareScopeResolution()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService<ITransactionDataProvider>().Returns(_transactionDataProvider);
            var serviceScope = Substitute.For<IServiceScope>();
            serviceScope.ServiceProvider.Returns(serviceProvider);
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            serviceScopeFactory.CreateScope().Returns(serviceScope);
            return serviceScopeFactory;
        }

        [Fact]
        public void CoinbaseClient_should_be_polled_frequently()
        {
            _coinbaseClient.ListTransactionsAsync().ReturnsForAnyArgs(ci =>
                new PagedResponse<Transaction> {Data = new Transaction[0]});
            
            var observer = SimulateObservationsDuringRunningTime();
            _coinbaseClient.ReceivedWithAnyArgs(10).ListTransactionsAsync();
            observer.DidNotReceiveWithAnyArgs().OnNext(default);
        }

        [Fact]
        public void TransactionStream_should_not_stop_if_coinbaseClient_return_error()
        {
            _coinbaseClient.ListTransactionsAsync().ThrowsForAnyArgs(new Exception());

            var observer = SimulateObservationsDuringRunningTime();
            _coinbaseClient.ReceivedWithAnyArgs(10).ListTransactionsAsync();

            observer.DidNotReceiveWithAnyArgs().OnNext(default);
        }

        [Fact]
        public void TransactionStream_should_not_show_duplicates()
        {
            var counter = 0;
            _coinbaseClient.GetCurrencyAsync("btc").ReturnsForAnyArgs(new Currency() { Decimals = 5, Symbol = "btc" });
            _coinbaseClient.ListTransactionsAsync().ReturnsForAnyArgs(ci =>
                new PagedResponse<Transaction>
                {
                    Data = new[] {new Transaction { Currency = "btc", Hashes = new[] {$"{counter++ % 3}"}}}
                });

            var observer = SimulateObservationsDuringRunningTime();

            observer.ReceivedWithAnyArgs(3).OnNext(default);
        }


        [Fact]
        public void DecimalCache_should_receive_call_during_stream()
        {
            _coinbaseClient.ListTransactionsAsync().ReturnsForAnyArgs(ci =>
                new PagedResponse<Transaction> { Data = new Transaction[]{new Transaction(){Currency = "btc"} } });
            
            SimulateObservationsDuringRunningTime();
            _cache.Received(10).TryGetValue("coinbase_decimal_btc", out var value );
        }

        [Fact]
        public void DecimalCache_should_call_CoinbaseApi_if_value_not_found()
        {
            _coinbaseClient.ListTransactionsAsync().ReturnsForAnyArgs(ci =>
                new PagedResponse<Transaction> { Data = new Transaction[] { new Transaction() { Currency = "btc" } } });
            _coinbaseClient.GetCurrencyAsync("btc").ReturnsForAnyArgs(new Currency() {Decimals = 5, Symbol = "btc"});
            SimulateObservationsDuringRunningTime();
            _coinbaseClient.Received(1).GetCurrencyAsync("btc");
        }
        
        private IObserver<Transaction> SimulateObservationsDuringRunningTime()
        {
            var observer = Substitute.For<IObserver<Transaction>>();
            _coinbaseListener.TransactionStream.Subscribe(observer);
            _testScheduler.Schedule(_runningTime, s => _coinbaseListener.StopListening());
            _testScheduler.Start();
            return observer;
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _coinbaseListener?.Dispose();
        }

        #endregion
    }
}