using System.ComponentModel.DataAnnotations;
using Nethereum.Util;

namespace Trakx.Common.Utils
{
    public class IsValidEthereumAddress : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            string strValue = value as string;
            if (!strValue.IsValidEthereumAddressHexFormat())
            {
                ErrorMessage = "Must be a valid ethereum address.";
                return false;
            }
            return true;
        }
    }
}

