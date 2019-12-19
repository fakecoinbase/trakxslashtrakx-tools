using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Trakx.Data.Market.Common.Extensions;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Server.Hubs
{
    public sealed class NavHubClient
    {
        private readonly IIndexDefinitionProvider _indexDefinitionProvider;

        public NavHubClient(IIndexDefinitionProvider indexDefinitionProvider)
        {
            _indexDefinitionProvider = indexDefinitionProvider;
        }
    }

    public sealed class NavHub : Hub
    {
        private readonly INavUpdater _navUpdater;
        private readonly IMemoryCache _memoryCache;

        public NavHub(INavUpdater navUpdater, IMemoryCache memoryCache)
        {
            _navUpdater = navUpdater;
            _memoryCache = memoryCache;
        }

        public async Task<bool> RegisterClientToNavUpdates(Guid clientId, Guid indexId)
        {
            if (!_memoryCache.TryGetValue<IndexDefinition>(indexId, out var index))
                return false;
            return await _navUpdater.RegisterToNavUpdates(clientId, index)
                .ConfigureAwait(false);
        }

        public bool DeregisterClientFromNavUpdates(Guid clientId, string symbol)
        {
            return _navUpdater.DeregisterFromNavUpdates(clientId, symbol);
        }

        public ChannelReader<NavUpdate> NavUpdatesStream()
        {
            return _navUpdater.NavUpdates.AsChannelReader(10);
        }
    }
}
