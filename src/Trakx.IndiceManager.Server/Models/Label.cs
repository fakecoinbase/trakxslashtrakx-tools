using System;
using System.Text.Json.Serialization;

namespace Trakx.IndiceManager.Server.Models
{
    public class Label
    {
        public Label(string key, string value, string scope, DateTimeOffset createdAt, DateTimeOffset updatedAt)
        {
            Key = key;
            Value = value;
            Scope = scope;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public Label() { }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
