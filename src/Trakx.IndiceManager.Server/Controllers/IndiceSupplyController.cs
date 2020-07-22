using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trakx.Common.Models;
using Trakx.IndiceManager.Server.Managers;
using IndiceSupplyTransactionModel = Trakx.IndiceManager.Server.Models.IndiceSupplyTransactionModel;

namespace Trakx.IndiceManager.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IndiceSupplyController : ControllerBase
    {
        private readonly IIndiceSupplyService _indiceSupplyService;

        public IndiceSupplyController(IIndiceSupplyService indiceSupplyService)
        {
            _indiceSupplyService = indiceSupplyService;
        }


        /// <summary>
        /// Tries to save a transaction of issuing or redeeming of indices in the database.
        /// </summary>
        /// <param name="transactionToSave">The transaction that we want to save.</param>
        /// <returns>An object with a response 201 if the adding was successful</returns>
        [HttpPost]
        public async Task<ActionResult<bool>> SaveTransaction([FromBody] IndiceSupplyTransactionModel transactionToSave)
        {
            if (string.IsNullOrWhiteSpace(transactionToSave.User))
                return BadRequest("There is no user attached to this transaction. Please try again.");

            if (!transactionToSave.IsValid())
                return BadRequest("Some mandatory parameters are missing, please try again.");

            var isSaved = await _indiceSupplyService.TryToSaveTransaction(transactionToSave);

            if (isSaved)
                return CreatedAtAction("The transaction has been added to the database", transactionToSave);

            return BadRequest("The addition in the database failed. Please try again.");
        }


        /// <summary>
        /// Tries to retrieve all of the issuing and redeeming of indices made by a specific user.
        /// </summary>
        /// <param name="userName">The user that made the transactions.</param>
        /// <returns>A list of <see cref="IndiceSupplyTransactionModel"/> with all the transactions.</returns>
        [HttpGet]
        public async Task<ActionResult<List<IndiceSupplyTransactionModel>>> RetrieveTransactions([FromBody] string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest("Can't retrieve all of the transaction because the user is undefined.");

            var transactions = await _indiceSupplyService.GetAllTransactionByUser(userName);

            if (transactions.Count == 0)
                return BadRequest("The user didn't make any transaction.");

            return Ok(transactions.Select(t => new IndiceSupplyTransactionModel(t)).ToList());
        }
    }
}