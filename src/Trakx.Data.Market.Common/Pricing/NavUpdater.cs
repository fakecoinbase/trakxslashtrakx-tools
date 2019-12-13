using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Common.Pricing
{
    public class NavUpdater : INavUpdater
    {
        private readonly INavCalculator _navCalculator;
        private readonly IIndexDefinitionProvider _indexProvider;
        private readonly ILogger<NavUpdater> _logger;

        private readonly Subject<NavUpdate> _subject;
        private readonly ConcurrentDictionary<string, IDisposable> _updateSubscriptions;

        public IObservable<NavUpdate> NavUpdates { get; }

        public NavUpdater(INavCalculator navCalculator, IIndexDefinitionProvider indexProvider, ILogger<NavUpdater> logger)
        {
            _navCalculator = navCalculator;
            _indexProvider = indexProvider;
            _logger = logger;

            _subject = new Subject<NavUpdate>();
            NavUpdates = _subject.AsObservable();
            _updateSubscriptions = new ConcurrentDictionary<string, IDisposable>();
        }

        public async Task RegisterToNavUpdates(string symbol)
        {
            var definitionFromSymbol = await _indexProvider.GetDefinitionFromSymbol(symbol);
            if(definitionFromSymbol == IndexDefinition.Default) return;

            var updateStream = Observable.Interval(TimeSpan.FromSeconds(2))
                .Select(async t =>
                {
                    decimal nav;
                    try
                    {
                        nav = await _navCalculator.CalculateCryptoCompareNav(symbol);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to calculate NAV for {0}", symbol);
                        nav = 0;
                    }
                    var update = new NavUpdate(symbol, nav);
                    _logger.LogDebug("Nav Updated: {0} - {1}", symbol, nav);
                    
                    return update;
                })
                .Select(calculationTask => calculationTask.ToObservable())
                .Concat();
            
            var subscription = updateStream
                .Do(n => _logger.LogDebug("Pushing {0}: {1} - {2}", n.TimeStamp, n.Symbol, n.Value))
                .Subscribe(_subject);

            if (_updateSubscriptions.TryAdd(symbol, subscription)) return;
            subscription.Dispose();
        }

        public void DeregisterFromNavUpdates(string symbol)
        {
            if(_updateSubscriptions.TryRemove(symbol, out var subscription)) subscription?.Dispose();
        }
    }
}
