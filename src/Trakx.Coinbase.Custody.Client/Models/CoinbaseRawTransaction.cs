using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Trakx.Coinbase.Custody.Client.Models
{

    public class CoinbaseRawTransaction
    {
        public CoinbaseRawTransaction(CoinbaseRawTransaction coinbaseRawTransaction)
        {
            Id = coinbaseRawTransaction.Id;
            Type = coinbaseRawTransaction.Type;
            State = coinbaseRawTransaction.State;
            Source = coinbaseRawTransaction.Source;
            Destination = coinbaseRawTransaction.Destination;
            UnscaledAmount = coinbaseRawTransaction.UnscaledAmount;
            Currency = coinbaseRawTransaction.Currency;
            Hashes = coinbaseRawTransaction.Hashes;
        }

        #nullable disable
        /// <summary>
        /// For serialisation only
        /// </summary>
        public CoinbaseRawTransaction() {}
        #nullable restore


        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Type of the transaction using <see cref="TransactionType"/>
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }


        /// <summary>
        /// State can be compared using <see cref="TransactionState"/>
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("amount")]
        [JsonProperty("amount")]
        public ulong UnscaledAmount { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("destination")]
        public string Destination { get; set; }

        [JsonPropertyName("created_at")]
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }


        [JsonPropertyName("updated_at")]
        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("wallet_id")]
        [JsonProperty("wallet_id")]
        public Guid WalletId { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("fee")]
        public long Fee { get; set; }

        [JsonPropertyName("hashes")]
        public string[] Hashes { get; set; }

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
