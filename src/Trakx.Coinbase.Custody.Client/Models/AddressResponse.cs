using System;
using System.Text.Json.Serialization;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class AddressResponse
    {
        #nullable disable
        [JsonPropertyName("address")]
        public string Address { get; set; }
        
        /// <summary>
        /// State can be compared using <see cref="AddressState"/>
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("balance")]
        public long Balance { get; set; }

        [JsonPropertyName("blockchain_link")]
        public Uri BlockchainLink { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }
        #nullable restore
    }

    public static class AddressState
    {
        /// <summary>
        /// Address is secured in offline storage
        /// </summary>
        public static readonly string Cold = "cold";

        /// <summary>
        /// 	Address is in-progress of being brought online
        /// </summary>
        public static readonly string RestoreInProgress = "restore_in_progress";

        /// <summary>
        /// 	Address has been brought online
        /// </summary>
        public static readonly string Restored = "restored";
    }
}
