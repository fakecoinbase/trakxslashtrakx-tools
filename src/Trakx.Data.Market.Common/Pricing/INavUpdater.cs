using System;
using System.Threading.Tasks;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Common.Pricing
{
    public interface INavUpdater
    {
        IObservable<NavUpdate> NavUpdates { get; }
        Task<bool> RegisterToNavUpdates(Guid clientId, IndexDefinition index);
        bool DeregisterFromNavUpdates(Guid clientId, string symbol);
    }
}