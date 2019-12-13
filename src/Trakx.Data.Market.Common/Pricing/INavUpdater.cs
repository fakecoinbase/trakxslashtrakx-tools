using System;
using System.Threading.Tasks;

namespace Trakx.Data.Market.Common.Pricing
{
    public interface INavUpdater
    {
        IObservable<NavUpdate> NavUpdates { get; }
        Task RegisterToNavUpdates(string symbol);
        void DeregisterFromNavUpdates(string symbol);
    }
}