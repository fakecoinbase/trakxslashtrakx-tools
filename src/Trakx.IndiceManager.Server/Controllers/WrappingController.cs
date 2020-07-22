using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Common.Models;
using Trakx.IndiceManager.Server.Managers;

namespace Trakx.IndiceManager.Server.Controllers
{
    /// <summary>
    /// Provides methods around non erc-20 wrapping, allowing to create wrapping/unwrapping transactions
    /// and checking the status of Trakx collateral reserves.
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class WrappingController : ControllerBase
    {
        private readonly IWrappingService _wrappingService;
        private readonly ICoinbaseClient _coinbaseClient;

        /// <inheritdoc />
        public WrappingController(IWrappingService wrappingService,
            ICoinbaseClient coinbaseClient)
        {
            _wrappingService = wrappingService;
            _coinbaseClient = coinbaseClient;
        }
        
        /// <summary>
        /// Allows to return a corresponding address to a token in order for the user to make the transfer to a specific address. 
        /// </summary>
        /// <param name="symbol">The symbol of the token for which we want Trakx' address.</param>
        /// <returns>The Trakx address associated to the <paramref name="symbol"/></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> GetTrakxAddressFromSymbol([FromBody]string symbol)
        {
            if (await _coinbaseClient.GetCurrencyAsync(symbol) == null)
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
        /// <returns>Return a BadRequest object if the information in <paramref name="transaction"/> are wrong.
        /// Else it return an OK object result.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<WrappingTransactionModel>>> GetAllTransactionByUser([FromBody] string user)
        {
            var transactions = await _wrappingService.GetTransactionByUser(user);

            if (transactions == null)
                return BadRequest("This User hasn't made any transactions.");

            return Ok(transactions.Select(t => new WrappingTransactionModel(t)).ToList());
        }

        /// <summary>
        /// This route allows to retrieve the Trakx' balance, either with native or wrapped tokens.
        /// </summary>
        /// <returns>An Ok object list composed by all the balances with status 200 or a InternalServerError object if it failed.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IAsyncEnumerable<AccountBalanceModel>>> 
            GetTrakxBalances(CancellationToken cancellationToken = default)
        {
            var balances = _wrappingService.GetTrakxBalances(cancellationToken);

            if (!await balances.AnyAsync(cancellationToken))
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "An error occurred, please try again.");

            return Ok(balances);
        }
    }
}