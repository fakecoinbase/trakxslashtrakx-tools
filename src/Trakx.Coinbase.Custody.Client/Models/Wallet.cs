using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class Wallet
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("balance")]
        public string Balance { get; set; }

        [JsonPropertyName("withdrawable_balance")]
        [JsonProperty("withdrawable_balance")]
        public string WithdrawableBalance { get; set; }

        [JsonProperty("created_at")]
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("cold_address")]
        [JsonProperty("cold_address")]
        public string ColdAddress { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }
}
