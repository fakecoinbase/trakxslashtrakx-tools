using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Util;
using Trakx.Common.Models;
using Trakx.IndiceManager.Server.Managers;

namespace Trakx.IndiceManager.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IndiceCreationController : Controller
    {
        private readonly IComponentInformationRetriever _componentRetriever;

        private readonly IIndiceInformationRetriever _indiceRetriever;

        private readonly IIndiceDatabaseWriter _indiceDatabaseWriter;

        public IndiceCreationController(IComponentInformationRetriever componentRetriever,IIndiceInformationRetriever indiceRetriever,IIndiceDatabaseWriter indiceDatabaseWriter)
        {
            _componentRetriever = componentRetriever;
            _indiceRetriever = indiceRetriever;
            _indiceDatabaseWriter = indiceDatabaseWriter;
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
        ///  Tries to retrieve all of the components that are currently in database.
        /// </summary>
        /// <returns>A list of <see cref="ComponentDetailModel"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<List<ComponentDetailModel>>> GetAllComponents()
        {
            var components = await _componentRetriever.GetAllComponents();

            if (components.Count == 0)
                return NotFound("There is no components in the database.");

            var returnComponents = components.Select(c => new ComponentDetailModel(c)).ToList();
            return Ok(returnComponents);
        }

        /// <summary>
        /// Tries to put a new component in the database to use it later in a new indice.
        /// </summary>
        /// <param name="componentDefinition">The component that we want to save.</param>
        /// <returns>An object with a response 201 if the adding was successful</returns>
        [HttpPost]
        public async Task<ActionResult<ComponentDetailModel>> SaveComponentDefinition(ComponentDetailModel componentDefinition)
        {
            if (!componentDefinition.IsValid())
                return BadRequest("The component is incomplete, please try again.");

            var isAdded = await _componentRetriever.TryToSaveComponentDefinition(componentDefinition);

            if (isAdded )
                return CreatedAtAction("The component has been added to the database.", componentDefinition);
            
            return BadRequest("Object already in database.");
        }


        /// <summary>
        /// Tries to retrieve all of the indices in our database.
        /// </summary>
        /// <returns>A list of indices with details about them.</returns>
        [HttpGet]
        public async Task<ActionResult<List<IndiceDetailModel>>> GetAllIndices()
        {
            var indiceDefinitions = await _indiceRetriever.GetAllIndicesFromDatabase();

            if (indiceDefinitions.Count==0)
                return NotFound("There is no indices in the database.");

            var indiceDetails = indiceDefinitions.Select(i => new IndiceDetailModel(i)).ToList();
            return Ok(indiceDetails);
        }


        /// <summary>
        /// Tries to get all of the compositions for an indice.
        /// </summary>
        /// <param name="symbol">The symbol of the indice that we want the compositions. Not to be confused with the composition symbol.</param>
        /// <returns>A list of the past and present composition for a given indice</returns>
        [HttpGet]
        public async Task<ActionResult<List<IndiceCompositionModel>>> GetCompositionsBySymbol([FromBody]string symbol)
        {
            var allCompositions = await _indiceRetriever.GetAllCompositionForIndiceFromDatabase(symbol);

            if (allCompositions == null)
                return NotFound($"The indice attached to {symbol} is not in our database.");

            if (allCompositions.Count == 0)
                return NotFound("There are no compositions for this indice.");

            return Ok(allCompositions.Select(i => new IndiceCompositionModel(i)).ToList());
        }


        /// <summary>
        /// Tries to save a new indice in our database.
        /// </summary>
        /// <param name="indiceToSave">The indice that we want to save.</param>
        /// <returns>An object with a response 201 if the adding was successful</returns>
        [HttpPost]
        public async Task<ActionResult<IndiceDetailModel>> SaveIndiceDefinition([FromBody]IndiceDetailModel indiceToSave)
        {
            if (!indiceToSave.Address.IsValidEthereumAddressHexFormat())
                return BadRequest($"{indiceToSave.Address} is not a valid ethereum address");

            if (await _indiceRetriever.SearchIndiceByAddress(indiceToSave.Address))
                return BadRequest("The indice is already in the database.");

            var result = await _indiceDatabaseWriter.TrySaveIndice(indiceToSave);
            if (result == true)
                return CreatedAtAction("The indice has been added to the database.", indiceToSave);

            return BadRequest("The addition in the database has failed. Please verify the parameters of the indice and try again.");
        }


        /// <summary>
        /// Tries to save a new composition for an indice in our database.
        /// </summary>
        /// <param name="compositionToSave">The composition that the user want to save.</param>
        /// <returns>An object with a response 201 if the adding was successful.</returns>
        [HttpPost]
        public async Task<ActionResult<IndiceCompositionModel>> SaveIndiceComposition([FromBody] IndiceCompositionModel compositionToSave)
        {
            if (!compositionToSave.Address.IsValidEthereumAddressHexFormat())
                return BadRequest($"{compositionToSave.Address} is not a valid ethereum address");

            if (await _indiceRetriever.SearchCompositionByAddress(compositionToSave.Address))
                return BadRequest("The composition is already in the database.");

            var result = await _indiceDatabaseWriter.TrySaveComposition(compositionToSave);

            if (result == true)
                return CreatedAtAction("The composition has been added to the database.", compositionToSave);

            return BadRequest("The addition in the database has failed. Please verify the parameters of the composition and try again.");
        }
    }
}
