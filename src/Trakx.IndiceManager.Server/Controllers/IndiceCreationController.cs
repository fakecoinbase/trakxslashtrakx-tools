using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Util;
using Trakx.Common.Interfaces.Indice;
using Trakx.IndiceManager.Server.Managers;
using Trakx.IndiceManager.Server.Models;
using Trakx.Persistence.DAO;

namespace Trakx.IndiceManager.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IndiceCreationController : Controller
    {
        private readonly IComponentInformationRetriever _componentRetriever;

        private readonly IIndiceInformationRetriever _indiceRetriever;

        public IndiceCreationController(IComponentInformationRetriever componentRetriever,IIndiceInformationRetriever indiceRetriever)
        {
            _componentRetriever = componentRetriever;
            _indiceRetriever = indiceRetriever;
        }
        
        /// <summary>
        /// Tries to retrieve a component and its details using its address on chain.
        /// </summary>
        /// <param name="address">The ethereum address of the component.</param>
        /// <returns>Details about the component at the given address, if they were found.</returns>
        [HttpGet]
        public async Task<ActionResult<ComponentDetailModel>> GetComponentByAddress([FromQuery]string address)
        {
            if (!address.IsValidEthereumAddressHexFormat())
                return BadRequest($"{address} is not a valid ethereum address");

            var details = await _componentRetriever.GetComponentDefinitionFromAddress(address);
            
            if (details == null)
                return NotFound($"Sorry {address} doesn't correspond to any ERC20 token.");
           
            var componentDetailModel = new ComponentDetailModel
            {
                Address = details.Address,
                Symbol = details.Symbol
            };
            return new JsonResult(componentDetailModel);
        }

        /// <summary>
        /// Tries to retrieve all of the indices in our database.
        /// </summary>
        /// <returns>A list of indices with details about them.</returns>
        [HttpGet]
        public async Task<ActionResult<List<IndiceDetailModel>>> GetAllIndices()
        {
            var list = await _indiceRetriever.GetAllIndicesFromDatabase();

            if (list == null)
                return NotFound("There is no indices in the database.");

            List<IndiceDetailModel> toReturn = new List<IndiceDetailModel>();
            foreach (IIndiceDefinition indice in list)
            {
                toReturn.Add(new IndiceDetailModel
                {
                    Symbol = indice.Symbol,
                    CreationDate = indice.CreationDate,
                    Name = indice.Name
                });
            }

            return Ok(toReturn);
        }
    }
}