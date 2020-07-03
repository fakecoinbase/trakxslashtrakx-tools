using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trakx.Common.Interfaces;

namespace Trakx.IndiceManager.Client
{
    public interface IAddressMapping
    {
        /// <summary>   
        /// Returns a small random amount that the user have to send in order to verify he owns the address.
        /// </summary>
        /// <param name="currencySymbol">symbol of the currency of the amount returned</param>
        /// <returns></returns>
        Task<decimal> GetVerificationAmount(string currencySymbol);

        /// <summary>
        /// Returns the trakx address for the currency in parameter.
        /// </summary>
        /// <param name="currencySymbol">symbol of the currency of the address returned</param>
        /// <returns></returns>
        Task<string> GetTrakxAddress(string currencySymbol);

        /// <summary>
        /// Returns the symbol of the currencies of all Trakx wallets in coinbase. 
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetAllSymbolAvailableOnCoinbase();

        /// <summary>
        /// Tell to the server to start verifying that the IUserAddress have send the amount to the correct address.
        /// </summary>
        /// <param name="userAddress">all the information about the mapping transaction</param>
        Task VerifyMappingTransaction(IUserAddress userAddress);
    }
}
