using System;
using Trakx.Common.Interfaces.Index;
using Trakx.Common.Pricing;

namespace Trakx.Common.Interfaces.Pricing
{
    public interface INavUpdater
    {
        IObservable<NavUpdate> NavUpdates { get; }
        bool RegisterToNavUpdates(Guid clientId, IIndexComposition index);
        bool DeregisterFromNavUpdates(Guid clientId, string symbol);
    }
}