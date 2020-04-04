using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.Messari.DTOs
{
    public partial class MiningStats
    {
        [JsonPropertyName("mining_algo")]
        public string MiningAlgo { get; set; }

        [JsonPropertyName("network_hash_rate")]
        public string NetworkHashRate { get; set; }

        [JsonPropertyName("available_on_nicehash_percent")]
        public double? AvailableOnNicehashPercent { get; set; }

        [JsonPropertyName("1_hour_attack_cost")]
        public double? The1_HourAttackCost { get; set; }

        [JsonPropertyName("24_hours_attack_cost")]
        public double? The24_HoursAttackCost { get; set; }

        [JsonPropertyName("attack_appeal")]
        public double? AttackAppeal { get; set; }
    }
}