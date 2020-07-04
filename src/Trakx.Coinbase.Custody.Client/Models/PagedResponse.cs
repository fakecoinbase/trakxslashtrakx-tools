using System.Text.Json.Serialization;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class PagedResponse<T>
    {
        #nullable disable
        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }


        [JsonPropertyName("data")]
        public T[] Data { get; set; }
        #nullable restore
    }
}