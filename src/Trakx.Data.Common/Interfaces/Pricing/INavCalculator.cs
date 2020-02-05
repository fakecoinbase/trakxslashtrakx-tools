using System;
using System.Threading.Tasks;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Common.Pricing;

namespace Trakx.Data.Common.Interfaces.Pricing
{
    public interface INavCalculator
    {
        Task<decimal> CalculateNav(IIndexComposition index, 
            DateTime? asOf = default, 
            string quoteCurrency = Constants.DefaultQuoteCurrency);

        Task<IIndexValuation> GetIndexValuation(IIndexComposition composition, 
            DateTime? asOf = default, 
            string quoteCurrency = Constants.DefaultQuoteCurrency);
    }
}