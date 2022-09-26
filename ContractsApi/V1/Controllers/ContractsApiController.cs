using ContractsApi.V1.Boundary.Requests;
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
        private readonly IGetContractByIdUseCase _getContractByIdUseCase;
        //private readonly IPostNewContractUseCase _postNewContractUseCase;
        public ContractsApiController(IGetContractByIdUseCase getContractByIdUseCase)
        {
            _getContractByIdUseCase = getContractByIdUseCase;
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
        public async Task<IActionResult> GetContractById([FromRoute] ContractQueryRequest query)
        {
            var result = await _getContractByIdUseCase.Execute(query).ConfigureAwait(false);
            if (result == null) return NotFound(query.Id);

            var eTag = string.Empty;
            if (result.VersionNumber.HasValue)
                eTag = result.VersionNumber.ToString();

            HttpContext.Response.Headers.Add(HeaderConstants.ETag, EntityTagHeaderValue.Parse($"\"{eTag}\"").Tag);

            return Ok(result);
        }
    }
}
