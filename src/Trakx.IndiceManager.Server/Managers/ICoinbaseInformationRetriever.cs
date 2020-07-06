using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trakx.IndiceManager.Server.Managers
{
    public interface ICoinbaseInformationRetriever
    {
        /// <summary>
        /// Get the trakx address according of the corresponding currency symbol.
        /// </summary>
        /// <param name="currencySymbol">Symbol of the currency</param>
        /// <returns>Trakx address of the symbol.</returns>
        Task<string> GetTrakxAddressBySymbol(string currencySymbol);

        /// <summary>
        /// Get all the currency symbols of trakx wallets.
        /// </summary>
        /// <returns>List of all available symbols.</returns>
        Task<List<string>> GetAllCurrencySymbols();

    }
}
