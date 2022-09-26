using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Infrastructure;
using ContractsApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ContractsApi.V1.Controllers
{
    [ApiController]
    //TODO: Rename to match the APIs endpoint
    [Route("api/v1/residents")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    //TODO: rename class to match the API name
    public class ContractsApiController : BaseController
    {
        private readonly IGetContractByIdUseCase _getByIdUseCase;
        //private readonly IPostNewContractUseCase _postNewContractUseCase;
        public ContractsApiController(IGetContractByIdUseCase getByIdUseCase)
        {
            _getByIdUseCase = getByIdUseCase;
        }

        /// <summary>
        /// Retrieves the asset with the supplied id
        /// </summary>
        /// <response code="200">Successfully retrieved details for the specified ID</response>
        /// <response code="404">No tenure information found for the specified ID</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(ContractResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> GetContractById([FromRoute] GetContractByIdRequest query)
        {
            var result = await _getAssetByIdUseCase.ExecuteAsync(query).ConfigureAwait(false);
            if (result == null) return NotFound(query.Id);

            var eTag = string.Empty;
            if (result.VersionNumber.HasValue)
                eTag = result.VersionNumber.ToString();

            HttpContext.Response.Headers.Add(HeaderConstants.ETag, EntityTagHeaderValue.Parse($"\"{eTag}\"").Tag);

            return Ok(result);
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-base-api/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObjectList), StatusCodes.Status200OK)]
        [HttpGet]
        [LogCall(LogLevel.Information)]

        public IActionResult ListContacts()
        {
            return Ok(_getAllUseCase.Execute());
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="404">No ? found for the specified ID</response>
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        //TODO: rename to match the identifier that will be used
        [Route("{yourId}")]
        public IActionResult ViewRecord(int yourId)
        {
            return Ok(_getByIdUseCase.Execute(yourId));
        }
    }
}
