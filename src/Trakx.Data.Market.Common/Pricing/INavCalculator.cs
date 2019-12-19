using System.Threading.Tasks;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Common.Pricing
{
    public interface INavCalculator
    {
        Task<decimal> CalculateNav(IndexDefinition index);
        Task<IndexPriced> GetIndexPriced(IndexDefinition index);
    }
}