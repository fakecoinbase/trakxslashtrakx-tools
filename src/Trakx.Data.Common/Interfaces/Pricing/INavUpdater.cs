using System;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Common.Pricing;

namespace Trakx.Data.Common.Interfaces.Pricing
{
    public interface INavUpdater
    {
        IObservable<NavUpdate> NavUpdates { get; }
        bool RegisterToNavUpdates(Guid clientId, IIndexComposition index);
        bool DeregisterFromNavUpdates(Guid clientId, string symbol);
    }
}