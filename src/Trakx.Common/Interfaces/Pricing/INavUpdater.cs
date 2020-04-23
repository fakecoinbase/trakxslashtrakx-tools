using System;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Pricing;

namespace Trakx.Common.Interfaces.Pricing
{
    public interface INavUpdater
    {
        IObservable<NavUpdate> NavUpdates { get; }
        bool RegisterToNavUpdates(Guid clientId, IIndiceComposition indice);
        bool DeregisterFromNavUpdates(Guid clientId, string symbol);
    }
}