using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

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
        public string WithdrawableBalance { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("cold_address")]
        public string ColdAddress { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }
}
