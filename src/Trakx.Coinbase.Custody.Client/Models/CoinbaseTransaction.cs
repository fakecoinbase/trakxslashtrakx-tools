using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Trakx.Coinbase.Custody.Client.Converter;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class CoinbaseTransaction
    {
        
#nullable disable
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public TransactionType Type { get; set; }

        [JsonProperty("state")]
        [JsonPropertyName("state")]
        public TransactionState State{ get; set; }

        [JsonProperty("amount")]
        [JsonPropertyName("amount")]
        public ulong UnscaledAmount { get; set; }

        [JsonProperty("destination")]
        [JsonPropertyName("destination")]
        public string? Destination { get; set; }

        [JsonProperty("created_at")]
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("wallet_id")]
        [JsonPropertyName("wallet_id")]
        public string WalletId { get; set; }

        [JsonProperty("amount_whole_units")]
        [JsonPropertyName("amount_whole_units")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonProperty("fee")]
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }

        [JsonProperty("hashes")]
        [JsonPropertyName("hashes")]
        public List<string> Hashes { get; set; }

        [JsonProperty("source")]
        [JsonPropertyName("source")]
        public string? Source { get; set; }
#nullable restore
    }

    public enum TransactionType
    {
        [Description(" On-chain deposit of funds to an associated address")]
        deposit,

        [Description("On-chain withdrawal of funds authorized by account consensus")]
        withdrawal,

        [Description("On-chain delegation of funds or votes to a destination address")]
        delegation,

        [Description("On-chain transaction to revoke a previous delegation of funds or votes")]
        undelegation,

        [Description("Registration of a crypto key for staking purposes")]
        key_registration,

        [Description("Reward payment to an associated address for a staked asset")]
        reward,

        [Description("On-chain deposit of funds into proxy contract from cold address")]
        proxy_deposit,

        [Description("On-chain withdrawal of funds from proxy contract to cold address")]
        proxy_withdrawal,

        [Description("Internal automated deposit to a cold address from a restored address")]
        sweep_deposit,

        [Description("Internal automated withdrawal from a restored address to a cold address")]
        sweep_withdrawal,

        [Description("Coinbase Custody automated invoice settlement payment")]
        billing_withdrawal,

        [Description("Coinbase Custody network fee payment for an eligible withdrawal")]
        coinbase_deposit,

        [Description("Coinbase Custody refund for the leftover amount for a CPFP (child pays for parent) transaction")]
        coinbase_refund
    }

    public enum TransactionState
    {
        [Description("Transaction has been created by an account member")]
        created,

        [Description("Transaction has been authorized by account consensus")]
        requested,

        [Description("Transaction has been authorized by Coinbase Custody")]
        approved,

        [Description("Transaction is pending coinbase_deposit fee funding")]
        gassing,

        [Description("Transaction is planned for broadcast")]
        planned,

        [Description("Transaction broadcast is pending secure address restore")]
        restored,

        [Description("Transaction has been broadcast and confirmed on-chain")]
        done,

        [Description("Transaction has been detected and credited")]
        imported,

        [Description("Transaction has been detected and credited")]
        canceled,

        [Description("Transaction has been rejected by an account member")]
        user_rejected,

        [Description("Transaction has been rejected by Coinbase Custody")]
        rejected
    }
}
