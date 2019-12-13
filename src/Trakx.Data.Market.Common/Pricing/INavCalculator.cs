using System.Threading.Tasks;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Common.Pricing
{
    public interface INavCalculator
    {
        Task<decimal> CalculateCryptoCompareNav(string index);
        Task<IndexPriced> GetIndexPricedByCryptoCompare(string indexSymbol);
    }
}