using System;
using System.Reactive.Concurrency;
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
        private readonly ITransactionDataProvider _transactionDataProvider;

        public CoinbaseTransactionListenerTests(ITestOutputHelper output)
        {
            _transactionDataProvider = Substitute.For<ITransactionDataProvider>();
            var serviceScopeFactory = PrepareScopeResolution();
            _coinbaseClient = Substitute.For<ICoinbaseClient>();
            _testScheduler = new TestScheduler();
            _runningTime = CoinbaseTransactionListener
                .PollingInterval.Multiply(10)
                .Add(TimeSpan.FromMilliseconds(100));
            _coinbaseListener = new CoinbaseTransactionListener(
                _coinbaseClient,
                output.ToLogger<CoinbaseTransactionListener>(), serviceScopeFactory, _testScheduler);
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
            _coinbaseClient.ListTransactionsAsync().ReturnsForAnyArgs(ci =>
                new PagedResponse<Transaction>
                {
                    Data = new[] {new Transaction {Hashes = new[] {$"{counter++ % 3}"}}}
                });

            var observer = SimulateObservationsDuringRunningTime();
            
            observer.ReceivedWithAnyArgs(3).OnNext(default);
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