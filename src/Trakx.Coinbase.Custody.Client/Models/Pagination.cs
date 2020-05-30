using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class Pagination
    {
        [JsonPropertyName("before")]
        public string Before { get; set; }

        [JsonPropertyName("after")]
        public string After { get; set; }
    }
}
