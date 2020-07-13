using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;
using Trakx.IndiceManager.Server.Models;

namespace Trakx.IndiceManager.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AddressMappingController : Controller
    {
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly IDepositorAddressRetriever _depositorAddressRetriever;

        public AddressMappingController(ICoinbaseClient coinbaseClient,
            IDepositorAddressRetriever depositorAddressRetriever)
        {
            _coinbaseClient = coinbaseClient;
            _depositorAddressRetriever = depositorAddressRetriever;
        }

        /// <summary>
        /// Get one trakx address on coinbase for currencySymbol.
        /// </summary>
        /// <param name="currencySymbol">Symbol of the currency in which the amount is calculated.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>Trakx address for currencySymbol.</returns>
        [HttpGet]
        public async Task<ActionResult<string>> GetTrakxAddress([FromQuery] string currencySymbol,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(currencySymbol))
                return BadRequest($"{currencySymbol} is null or empty");

            var address = await _coinbaseClient.GetWallets(currencySymbol, cancellationToken: cancellationToken)
                .FirstOrDefaultAsync(cancellationToken);

            if (address == null)
                return NotFound($"Sorry {currencySymbol} doesn't have any corresponding address on trakx wallet.");

            return Ok(address.ColdAddress);
        }

        /// <summary>
        /// Get symbols of all trakx wallets available on coinbase.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>List of symbols.</returns>
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetAllSymbolAvailableOnCoinbase(
        CancellationToken cancellationToken = default)
        {
            var symbolList = _coinbaseClient.GetCurrencies(cancellationToken: cancellationToken);

            if (symbolList == null || !(await symbolList.AnyAsync(cancellationToken)))
                return NotFound("Sorry, impossible to retrieve all currency symbols on Coinbase.");

            return Ok(symbolList.Select(c => c.Symbol));
        }

        /// <summary>
        /// Links a user to an address with a verification amount. Once the transfer has been sent to the
        /// address, the <paramref name="claimedAddress"/> will be set to verified, and the caller of the
        /// method will be the owner.
        /// </summary>
        /// <param name="claimedAddress">The address for which the ownership is claimed.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IDepositorAddress>> RegisterUserAsAddressOwner(
            [FromBody] DepositAddressModel claimedAddress,
            CancellationToken cancellationToken = default)
        {
            var validationContext = new ValidationContext(claimedAddress);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(claimedAddress, validationContext, validationResults, true))
                return BadRequest("Invalid deposit address, please try again." + Environment.NewLine +
                                  string.Join(Environment.NewLine, validationResults.Select(v => v.ErrorMessage).ToList()));

            var depositorAddressId = DepositorAddressExtension.GetDepositorAddressId(claimedAddress.CurrencySymbol, claimedAddress.Address);
            var existingAddress = await _depositorAddressRetriever.GetDepositorAddressById(depositorAddressId, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (existingAddress?.IsVerified ?? false)
                return BadRequest("This address has already been verified.");

            var depositAddress = existingAddress ?? claimedAddress.ToDepositAddress();

            var currency = await _coinbaseClient.GetCurrencyAsync(claimedAddress.CurrencySymbol, cancellationToken)
                .ConfigureAwait(false);
            var decimals = currency.Decimals;

            var candidate = new User("blablablassfrr", new List<IDepositorAddress>());
            var candidateRegistered = await _depositorAddressRetriever.AssociateCandidateUser(depositAddress, candidate, decimals,
                cancellationToken).ConfigureAwait(false);

            if (!candidateRegistered)
                return BadRequest("The addition in the database has failed. " +
                                  "Please verify the parameters of the indice and try again.");

            var updatedDepositAddress = await
                _depositorAddressRetriever.GetDepositorAddressById(depositAddress.Id, true, cancellationToken)
                    .ConfigureAwait(false);

            return Accepted(updatedDepositAddress);
        }
    }
}
