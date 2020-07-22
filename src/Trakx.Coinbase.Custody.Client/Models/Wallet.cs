using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Trakx.Coinbase.Custody.Client.Converter;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class Wallet
    {
#nullable disable
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonProperty("created_at")]
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(NewtonSoftStringToULongConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(SystemStringToULongConverter))]
        [JsonProperty("balance")]
        [JsonPropertyName("balance")]
        public ulong UnscaledBalance { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(NewtonSoftStringToDecimalConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(SystemStringToDecimalConverter))]
        [JsonProperty("balance_whole_units")]
        [JsonPropertyName("balance_whole_units")]
        public decimal Balance { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(NewtonSoftStringToULongConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(SystemStringToULongConverter))]
        [JsonProperty("withdrawable_balance")]
        [JsonPropertyName("withdrawable_balance")]
        public ulong UnscaledWithdrawableBalance { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(NewtonSoftStringToDecimalConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(SystemStringToDecimalConverter))]
        [JsonProperty("withdrawable_balance_whole_units")]
        [JsonPropertyName("withdrawable_balance_whole_units")]
        public decimal WithdrawableBalance { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(NewtonSoftStringToULongConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(SystemStringToULongConverter))]
        [JsonProperty("unvested_balance")]
        [JsonPropertyName("unvested_balance")]
        public ulong UnscaledUnvestedBalance { get; set; }


        [Newtonsoft.Json.JsonConverter(typeof(NewtonSoftStringToDecimalConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(SystemStringToDecimalConverter))]
        [JsonProperty("unvested_balance_whole_units")]
        [JsonPropertyName("unvested_balance_whole_units")]
        public decimal UnvestedBalance { get; set; }

        [JsonProperty("cold_address")]
        [JsonPropertyName("cold_address")]
        public string ColdAddress { get; set; }

        [JsonProperty("currency")]
        [JsonPropertyName("currency")]
        public string CurrencySymbol { get; set; }

#nullable restore
    }
}
