using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Trakx.Coinbase.Custody.Client.Models
{
    
    public class Transaction
    {
        
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
        public long Amount { get; set; }

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

    public static class TransactionType
    {
        /// <summary>
        /// On-chain deposit of funds to an associated address
        /// </summary>
        public static readonly string Deposit = "deposit";

        /// <summary>
        /// On-chain withdrawal of funds authorized by account consensus
        /// </summary>
        public static readonly string Withdrawal = "withdrawal";

        /// <summary>
        /// On-chain delegation of funds or votes to a destination address
        /// </summary>
        public static readonly string Delegation = "delegation";

        /// <summary>
        /// On-chain transaction to revoke a previous delegation of funds or votes
        /// </summary>
        public static readonly string Undelegation = "undelegation";

        /// <summary>
        /// Registration of a crypto key for staking purposes
        /// </summary>
        public static readonly string KeyRegistration = "key_registration";

        /// <summary>
        /// Reward payment to an associated address for a staked asset
        /// </summary>
        public static readonly string Reward = "reward";

        /// <summary>
        /// On-chain deposit of funds into proxy contract from cold address
        /// </summary>
        public static readonly string ProxyDeposit = "proxy_deposit";

        /// <summary>
        /// On-chain withdrawal of funds from proxy contract to cold address
        /// </summary>
        public static readonly string ProxyWithdrawal = "proxy_withdrawal";

        /// <summary>
        /// Internal automated deposit to a cold address from a restored address
        /// </summary>
        public static readonly string SweepDeposit = "sweep_deposit";

        /// <summary>
        /// Internal automated withdrawal from a restored address to a cold address
        /// </summary>
        public static readonly string SweepWithdrawal = "sweep_withdrawal";

        /// <summary>
        /// Coinbase Custody automated invoice settlement payment
        /// </summary>
        public static readonly string BillingWithdrawal = "billing_withdrawal";

        /// <summary>
        /// Coinbase Custody network fee payment for an eligible withdrawal
        /// </summary>
        public static readonly string CoinbaseDeposit = "coinbase_deposit";

        /// <summary>
        /// Coinbase Custody refund for the leftover amount for a CPFP (child pays for parent) transaction
        /// </summary>
        public static readonly string CoinbaseRefund = "coinbase_refund";
    }

    public static class TransactionState
    {
        /// <summary>
        /// Transaction has been created by an account member
        /// </summary>
        public static readonly string Created = "created";

        /// <summary>
        /// Transaction has been authorized by account consensus
        /// </summary>
        public static readonly string Requested = "requested";

        /// <summary>
        /// Transaction has been authorized by Coinbase Custody
        /// </summary>
        public static readonly string Approved = "approved";

        /// <summary>
        /// Transaction is pending coinbase_deposit fee funding
        /// </summary>
        public static readonly string Gassing = "gassing";

        /// <summary>
        /// Transaction is planned for broadcast
        /// </summary>
        public static readonly string Planned = "planned";

        /// <summary>
        /// Transaction broadcast is pending secure address restore
        /// </summary>
        public static readonly string Restored = "restored";

        /// <summary>
        /// Transaction has been broadcast and confirmed on-chain
        /// </summary>
        public static readonly string Done = "done";

        /// <summary>
        /// Transaction has been detected and credited
        /// </summary>
        public static readonly string Imported = "imported";

        /// <summary>
        /// Transaction has been cancelled
        /// </summary>
        public static readonly string Canceled = "canceled";

        /// <summary>
        /// Transaction has been rejected by an account member
        /// </summary>
        public static readonly string UserRejected = "user_rejected";

        /// <summary>
        /// Transaction has been rejected by Coinbase Custody
        /// </summary>
        public static readonly string Rejected = "rejected";
    }


}
