﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Pricing;
using Guid = System.Guid;

namespace Trakx.Common.Pricing
{
    public class NavUpdater : INavUpdater
    {
        private sealed class UpdatesWithListeners : IDisposable
        {
            public UpdatesWithListeners(IObservable<NavUpdate> updates, 
                ConcurrentDictionary<Guid, bool> listeners,
                CancellationTokenSource cancellationTokenSource)
            {
                _updates = updates;
                CancellationTokenSource = cancellationTokenSource;
                Listeners = listeners;
            }

            private readonly IObservable<NavUpdate> _updates;
            public CancellationTokenSource CancellationTokenSource { get; }
            public ConcurrentDictionary<Guid, bool> Listeners { get; }

            public void Dispose()
            {
                CancellationTokenSource?.Dispose();
            }
        }

        private readonly INavCalculator _navCalculator;
        private readonly ILogger<NavUpdater> _logger;

        private readonly Subject<NavUpdate> _subject;
        private readonly ConcurrentDictionary<string, UpdatesWithListeners> _priceUpdatesBySymbol;

        public IObservable<NavUpdate> NavUpdates { get; }

        public NavUpdater(INavCalculator navCalculator, 
            ILogger<NavUpdater> logger)
        {
            _navCalculator = navCalculator;
            _logger = logger;

            _subject = new Subject<NavUpdate>();
            NavUpdates = _subject.AsObservable();
            
            _priceUpdatesBySymbol = new ConcurrentDictionary<string, UpdatesWithListeners>();
        }

        public bool RegisterToNavUpdates(Guid clientId, IIndiceComposition indice)
        {
            var updates = _priceUpdatesBySymbol.GetOrAdd(indice.IndiceDefinition.Symbol,
                s => AddUpdatesToMainStream(indice));

            return updates.Listeners.TryAdd(clientId, true);
        }

        private UpdatesWithListeners AddUpdatesToMainStream(IIndiceComposition indice)
        {
            var updates = CreateUpdateStreamForSymbol(indice);
            updates.UpdateStream.Subscribe(_subject.OnNext, 
                _subject.OnError,
                () =>
                {
                    _logger.LogDebug($"Updates for {indice.IndiceDefinition.Symbol} have stopped.");
                });
            var updatesWithListeners = 
                new UpdatesWithListeners(updates.UpdateStream, 
                    new ConcurrentDictionary<Guid, bool>(),
                    updates.CancellationTokenSource);

            return updatesWithListeners;
        }

        private (CancellationTokenSource CancellationTokenSource, IObservable<NavUpdate> UpdateStream)
            CreateUpdateStreamForSymbol(IIndiceComposition indice)
        {
            var cts = new CancellationTokenSource();
            var updateStream = Observable.Interval(TimeSpan.FromSeconds(2))
                .Select(async t =>
                {
                    decimal nav;
                    try
                    {
                        nav = await _navCalculator.CalculateNav(indice);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to calculate NAV for {0}", indice?.IndiceDefinition.Symbol ?? "N/A");
                        nav = 0;
                    }

                    var update = new NavUpdate(indice.IndiceDefinition.Symbol, nav);
                    _logger.LogDebug("Nav Updated: {0} - {1}", indice.IndiceDefinition.Symbol, nav);

                    return update;
                })
                .Select(calculationTask => calculationTask.ToObservable())
                .Concat()
                .Do(n => _logger.LogTrace( "Pushing {0}: {1} - {2}", n.TimeStamp, n.Symbol, n.Value))
                .TakeUntil(_ => cts.IsCancellationRequested);

            return (cts, updateStream);
        }

        public bool DeregisterFromNavUpdates(Guid clientId, string symbol)
        {
            if (!_priceUpdatesBySymbol.TryGetValue(symbol, out var subscriptions))
                return false;

            lock (subscriptions)
            {
                var removed = subscriptions.Listeners.TryRemove(clientId, out var _);
                if (subscriptions.Listeners.Any()) return removed;

                _priceUpdatesBySymbol.TryRemove(symbol, out var _);
                subscriptions.CancellationTokenSource.Cancel();
                subscriptions.Dispose();
                return removed;
            }
        }
    }
}