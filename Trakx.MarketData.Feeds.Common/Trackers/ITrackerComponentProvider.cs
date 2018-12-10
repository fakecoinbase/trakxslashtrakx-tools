using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trakx.MarketData.Feeds.Common.Trackers
{
    public interface ITrackerComponentProvider
    {
        Task<IList<string>> GetComponentTickers(string trackerTicker);
    }
}
