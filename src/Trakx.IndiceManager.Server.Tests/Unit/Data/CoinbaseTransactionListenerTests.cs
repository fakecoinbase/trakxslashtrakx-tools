using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using FluentAssertions.Extensions;
using Flurl.Http;
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
    public class CoinbaseTransactionListenerTests
    {
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly CoinbaseTransactionListener _coinbaseListener;
        private readonly TestScheduler _testScheduler;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TimeSpan _runningTime;

        public CoinbaseTransactionListenerTests(ITestOutputHelper output)
        {
            _coinbaseClient = Substitute.For<ICoinbaseClient>();
            _testScheduler = new TestScheduler();
            _runningTime = CoinbaseTransactionListener
                .PollingInterval.Multiply(10)
                .Add(TimeSpan.FromMilliseconds(100));
            _cancellationTokenSource = new CancellationTokenSource(_runningTime);
            _coinbaseListener = new CoinbaseTransactionListener(
                _coinbaseClient,
                output.ToLogger<CoinbaseTransactionListener>(),
                Substitute.For<ITransactionDataProvider>(), _testScheduler);
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
    }
}