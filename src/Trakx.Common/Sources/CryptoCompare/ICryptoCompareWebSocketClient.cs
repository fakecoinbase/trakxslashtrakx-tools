using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Trakx.Common.Sources.CryptoCompare
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