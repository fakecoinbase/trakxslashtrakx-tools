using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly ICurrencyCache _cache;
        private readonly ITransactionDataProvider _transactionDataProvider;

        public CoinbaseTransactionListenerTests(ITestOutputHelper output)
        {
            _transactionDataProvider = Substitute.For<ITransactionDataProvider>();
            _transactionDataProvider.GetLastWrappingTransactionDatetime().ReturnsForAnyArgs(new DateTime(2020, 12, 12));

            var serviceScopeFactory = PrepareScopeResolution();
            _coinbaseClient = Substitute.For<ICoinbaseClient>();
            _cache = Substitute.For<ICurrencyCache>();
            _testScheduler = new TestScheduler();
            _runningTime = CoinbaseTransactionListener
                .PollingInterval.Multiply(10)
                .Add(TimeSpan.FromMilliseconds(100));
            _coinbaseListener = new CoinbaseTransactionListener(
                _coinbaseClient, serviceScopeFactory, _cache, output.ToLogger<CoinbaseTransactionListener>(), _testScheduler);
        }

        private IServiceScopeFactory PrepareScopeResolution()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService<ITransactionDataProvider>()
                .ReturnsForAnyArgs(_transactionDataProvider);
            var serviceScope = Substitute.For<IServiceScope>();
            serviceScope.ServiceProvider.Returns(serviceProvider);
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            serviceScopeFactory.CreateScope().Returns(serviceScope);
            return serviceScopeFactory;
        }

        [Fact]
        public void CoinbaseClient_should_be_polled_frequently()
        {
            var observer = SimulateObservationsDuringRunningTime();
            _coinbaseClient.ReceivedWithAnyArgs(10).GetTransactions();
            observer.DidNotReceiveWithAnyArgs().OnNext(default);
        }

        [Fact]
        public void TransactionStream_should_not_stop_if_coinbaseClient_return_error()
        {
            _coinbaseClient.GetTransactions().ThrowsForAnyArgs(new Exception());

            var observer = SimulateObservationsDuringRunningTime();
            _coinbaseClient.ReceivedWithAnyArgs(10).GetTransactions();

            observer.DidNotReceiveWithAnyArgs().OnNext(default);
        }

        [Fact]
        public void TransactionStream_should_not_show_duplicates()
        {
            var transactionEnumerable = new List<CoinbaseRawTransaction>
            {
                new CoinbaseRawTransaction {Currency = "btc", Hashes = new[] { "abc" }},
                new CoinbaseRawTransaction {Currency = "btc", Hashes = new[] { "def" }},
                new CoinbaseRawTransaction {Currency = "btc", Hashes = new[] { "def" }}
            };

            _coinbaseClient.GetTransactions().ReturnsForAnyArgs(transactionEnumerable.ToAsyncEnumerable());
            _cache.GetDecimalsForCurrency("btc").ReturnsForAnyArgs((ushort)5);
            
            var observer = SimulateObservationsDuringRunningTime();

            observer.ReceivedWithAnyArgs(2).OnNext(default);
        }

        [Fact]
        public void TransactionStream_should_not_stop_when_currency_cannot_be_retrieved()
        {
            var transactionEnumerable = new List<CoinbaseRawTransaction>
            {
                new CoinbaseRawTransaction {Currency = "btc", Hashes = new[] { "abc" }},
                new CoinbaseRawTransaction {Currency = "btc", Hashes = new[] { "def" }},
                new CoinbaseRawTransaction {Currency = "eth", Hashes = new[] { "ght" }}
            };
            _coinbaseClient.GetTransactions().ReturnsForAnyArgs(transactionEnumerable.ToAsyncEnumerable());

            _cache.GetDecimalsForCurrency("btc").Returns((ushort?)null);
            _cache.GetDecimalsForCurrency("eth").Returns((ushort)10);

            var observer = SimulateObservationsDuringRunningTime();
            observer.ReceivedWithAnyArgs(1);
        }


        private IObserver<CoinbaseTransaction> SimulateObservationsDuringRunningTime()
        {
            var observer = Substitute.For<IObserver<CoinbaseTransaction>>();
            _coinbaseListener.TransactionStream.Subscribe(observer);
            _testScheduler.Schedule(_runningTime, s => _coinbaseListener.StopListening());
            _testScheduler.Start();
            _coinbaseClient.ReceivedWithAnyArgs(10).GetTransactions();
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