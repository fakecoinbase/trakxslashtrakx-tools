using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Trakx.Data.Common.Sources.CryptoCompare
{
    public interface ICryptoCompareWebSocketClient
    {
        IWebSocketStreamer WebSocketStreamer { get; }
        WebSocketState State { get; }
        TaskStatus? ListenInboundMessagesTaskStatus { get; }
        Task Connect();
        Task AddSubscriptions(params ICryptoCompareSubscription[] subscriptions);
        Task RemoveSubscriptions(params ICryptoCompareSubscription[] subscriptions);
        Task Disconnect();
    }
}