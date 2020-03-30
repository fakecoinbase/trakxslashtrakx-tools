using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Trakx.Data.Common.Sources.CryptoCompare.DTOs;

namespace Trakx.Data.Common.Sources.CryptoCompare
{
    public interface IWebSocketStreamer
    {
        IObservable<WebSocketInboundMessage> AllInboundMessagesStream { get; }
        IObservable<AggregateIndexResponse> AggregateIndexStream { get; }
        IObservable<SubscribeCompleteMessage> SubscribeCompleteStream { get; }
        IObservable<UnsubscribeCompleteMessage> UnsubscribeCompleteStream { get; }
        IObservable<LoadCompleteMessage> LoadCompleteStream { get; }
        IObservable<UnsubscribeAllCompleteMessage> UnsubscribeAllCompleteStream { get; }
        IObservable<HeartBeatMessage> HeartBeatStream { get; }
        void PublishInboundMessageOnStream(string rawMessage);
    }

    public class WebSocketStreamer : IWebSocketStreamer
    {
        private readonly ILogger<WebSocketStreamer> _logger;
        private readonly ISubject<WebSocketInboundMessage> _incomingMessageSubject;
        
        public WebSocketStreamer(ILogger<WebSocketStreamer> logger)
        {
            _logger = logger;
            _incomingMessageSubject = new ReplaySubject<WebSocketInboundMessage>(1);
        }

        public IObservable<WebSocketInboundMessage> AllInboundMessagesStream => _incomingMessageSubject.AsObservable();
        public IObservable<AggregateIndexResponse> AggregateIndexStream => _incomingMessageSubject.Cast<AggregateIndexResponse>().AsObservable();
        public IObservable<SubscribeCompleteMessage> SubscribeCompleteStream => _incomingMessageSubject.Cast<SubscribeCompleteMessage>().AsObservable();
        public IObservable<UnsubscribeCompleteMessage> UnsubscribeCompleteStream => _incomingMessageSubject.Cast<UnsubscribeCompleteMessage>().AsObservable();
        public IObservable<LoadCompleteMessage> LoadCompleteStream => _incomingMessageSubject.Cast<LoadCompleteMessage>().AsObservable();
        public IObservable<UnsubscribeAllCompleteMessage> UnsubscribeAllCompleteStream => _incomingMessageSubject.Cast<UnsubscribeAllCompleteMessage>().AsObservable();
        public IObservable<HeartBeatMessage> HeartBeatStream => _incomingMessageSubject.Cast<HeartBeatMessage>().AsObservable();

        public void PublishInboundMessageOnStream(string rawMessage)
        {
            try
            {
                _logger.LogTrace("Received WebSocketInboundMessage {0}{1}", Environment.NewLine, rawMessage);
                var message = JsonSerializer.Deserialize<WebSocketInboundMessage>(rawMessage);
                switch (message.Type)
                {
                    case AggregateIndexResponse.TypeValue:
                        _incomingMessageSubject.OnNext(JsonSerializer.Deserialize<AggregateIndexResponse>(rawMessage));
                        break;
                    case SubscribeCompleteMessage.TypeValue:
                        _incomingMessageSubject.OnNext(JsonSerializer.Deserialize<SubscribeCompleteMessage>(rawMessage));
                        break;
                    case UnsubscribeCompleteMessage.TypeValue:
                        _incomingMessageSubject.OnNext(JsonSerializer.Deserialize<SubscribeCompleteMessage>(rawMessage));
                        break;
                    case LoadCompleteMessage.TypeValue:
                        _incomingMessageSubject.OnNext(JsonSerializer.Deserialize<LoadCompleteMessage>(rawMessage));
                        break;
                    case UnsubscribeAllCompleteMessage.TypeValue:
                        _incomingMessageSubject.OnNext(JsonSerializer.Deserialize<SubscribeCompleteMessage>(rawMessage));
                        break;
                    case HeartBeatMessage.TypeValue:
                        _incomingMessageSubject.OnNext(JsonSerializer.Deserialize<HeartBeatMessage>(rawMessage));
                        break;
                    case ErrorMessage.TypeValue:
                        _incomingMessageSubject.OnNext(JsonSerializer.Deserialize<ErrorMessage>(rawMessage));
                        break;
                    default:
                        return;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to publish {0}", rawMessage);
            }
        }
    }
}