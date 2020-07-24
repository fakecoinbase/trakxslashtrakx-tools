using System.ComponentModel.DataAnnotations;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;

namespace Trakx.IndiceManager.Server.Models
{
    public class ExternalAddressModel
    {
        [Required, RegularExpression(@"[\w]{2,}")]
        public string CurrencySymbol { get; set; }

        [Required, RegularExpression(@"[\w]{10,}")]
        public string Address { get; set; }

        public IExternalAddress ToExternalAddress()
        {
            return new ExternalAddress(Address, CurrencySymbol);
        }
    }
}