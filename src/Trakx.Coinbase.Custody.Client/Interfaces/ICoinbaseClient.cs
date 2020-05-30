using System.Collections.Generic;
using System.Net.Http;
using Flurl.Http;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    public interface ICoinbaseClient
    {
        IFlurlRequest Request(params object[] urlSegments);
        string BaseUrl { get; set; }
        IDictionary<string, object> Headers { get; }
        HttpClient HttpClient { get; }
    }
}