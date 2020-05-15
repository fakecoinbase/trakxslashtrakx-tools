using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trakx.Common.Models;
using Trakx.Common.Sources.Coinbase;
using Trakx.IndiceManager.Server.Managers;

namespace Trakx.IndiceManager.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WrappingController : ControllerBase
    {
        private readonly IWrappingService _wrappingService;
        private readonly ICoinbaseClient _coinbaseClient;

        public WrappingController(IWrappingService wrappingService,ICoinbaseClient coinbaseClient)
        {
            _wrappingService = wrappingService;
            _coinbaseClient = coinbaseClient;
        }
            /// <summary>
        /// Allows to return a corresponding address to a token in order for the user to make the transfer to a specific address. 
        /// </summary>
        /// <param name="symbol">The symbol of the token for which we want the Trakx's address.</param>
        /// <returns>The Trakx address associated to the <paramref name="symbol"/></returns>
        [HttpGet]
        public async Task<ActionResult<string>> GetTrakxAddressFromSymbol([FromBody]string symbol)
        {
            if (!_coinbaseClient.CustodiedCoins.Contains(symbol))
                return NotFound("Coinbase Custody don't have this type of token.");

            var address = await _wrappingService.RetrieveAddressFromSymbol(symbol);

            if (address == null)
                return BadRequest("Sorry we can't find the address, please try again.");

            return Ok(address);
        }


        /// <summary>
        /// This route allow to wrap and unwrap tokens. It will register the transaction in the database and send the correct
        /// amount of tokens/wrapped tokens to the user.
        /// </summary>
        /// <param name="transaction">The <see cref="WrappingTransactionModel"/> which that has all the information needed to
        /// complete the transaction.</param>
        /// <returns>Return a BadRequest object if the informations in <paramref name="transaction"/> are wrong.
        /// Else it return an OK object result.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<string>> WrapTokens([FromBody] WrappingTransactionModel transaction)
        {
            var transactionHash = await _wrappingService.TryToFindTransaction(transaction);

            if (transactionHash == null)
                return BadRequest("Transaction Failed, please try again.");

            await _wrappingService.InitiateWrapping(transactionHash);

            return Ok("Transaction succeed.");
        }


        /// <summary>
        /// This route allow to retrieve all the transaction associated to an user.
        /// </summary>
        /// <param name="user">Here is the name of the user that is register in all the transactions.</param>
        /// <returns>All the transaction or a BadRequest object if the user doesn't have any transaction associated.</returns>
        [HttpGet]
        public async Task<ActionResult<List<WrappingTransactionModel>>> GetAllTransactionByUser([FromBody] string user)
        {
            var transactions = await _wrappingService.GetTransactionByUser(user);

            if (transactions == null)
                return BadRequest("This User hasn't made any transactions.");

            return Ok(transactions.Select(t => new WrappingTransactionModel(t)).ToList());
        }

        /// <summary>
        /// This route allows to retrieve the Trakx's balance, either with native or wrapped tokens.
        /// </summary>
        /// <returns>An Ok object list composed by all the balances with status 200 or a BadRequest Object if it failed.</returns>
        [HttpGet]
        public async Task<ActionResult<List<AccountBalanceModel>>> GetTrakxBalance()
        {
            var balances = await _wrappingService.GetBalances();

            if (balances == null)
                return BadRequest("An error occurred, please try again.");

            return Ok(balances);
        }
    }
}