using System;
using System.ComponentModel.DataAnnotations;

namespace Trakx.IndiceManager.ApiClient
{
    public class IndexManagerApiConfiguration
    {
        [Required]
        public string BaseUrl { get; set; }
    }
}
