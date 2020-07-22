using System.ComponentModel.DataAnnotations;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;

namespace Trakx.Common.Models
{
    public class DepositAddressModel
    {
        [Required, RegularExpression(@"[\w]{2,}")]
        public string CurrencySymbol { get; set; }

        [Required, RegularExpression(@"[\w]{10,}")]
        public string Address { get; set; }

        public IDepositorAddress ToDepositAddress()
        {
            return new DepositorAddress(Address, CurrencySymbol);
        }
    }
}