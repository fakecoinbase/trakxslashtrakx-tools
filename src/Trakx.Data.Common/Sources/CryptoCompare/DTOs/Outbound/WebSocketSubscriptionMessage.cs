using System.Collections.Generic;
using System.Text.Json.Serialization;
using Trakx.Data.Common.Sources.CryptoCompare.DTOs.Inbound;

namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs.Outbound
{
    public class WebSocketSubscriptionMessage
    {
        protected internal WebSocketSubscriptionMessage(string action, string format = "streamer")
        {
            Action = action;
            Format = format;
        }
        
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("subs")]
        public List<string> Subscriptions { get; } = new List<string>();

        [JsonPropertyName("format")] 
        public string Format { get; set; }
    }

    public sealed class AddSubscriptionMessage : WebSocketSubscriptionMessage
    {
        public const string SubAdd = "SubAdd";

        /// <inheritdoc />
        public AddSubscriptionMessage(string format = "streamer") : base(SubAdd, format) {}
    }

    public sealed class RemoveSubscriptionMessage : WebSocketSubscriptionMessage
    {
        public const string SubRemove = "SubRemove";

        /// <inheritdoc />
        public RemoveSubscriptionMessage(string format = "streamer") : base(SubRemove, format) { }
    }
}