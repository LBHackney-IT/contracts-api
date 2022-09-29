using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Infrastructure;
using ContractsApi.V1.UseCase.Interfaces;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HeaderConstants = ContractsApi.V1.Infrastructure.HeaderConstants;

namespace ContractsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/contracts")]
    [Produces("application/json")]
    [ApiVersion("1.0")]

    public class ContractsApiController : BaseController
    {
        private readonly IGetContractByIdUseCase _getContractByIdUseCase;
        private readonly IPostNewContractUseCase _postNewContractUseCase;
        private readonly ITokenFactory _tokenFactory;
        private readonly IHttpContextWrapper _contextWrapper;

        public ContractsApiController(IGetContractByIdUseCase getContractByIdUseCase,
            IPostNewContractUseCase postNewContractUseCase, ITokenFactory tokenFactory,
            IHttpContextWrapper contextWrapper)
        {
            _getContractByIdUseCase = getContractByIdUseCase;
            _postNewContractUseCase = postNewContractUseCase;

            _tokenFactory = tokenFactory;
            _contextWrapper = contextWrapper;
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

        [ProducesResponseType(typeof(ContractResponseObject), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> PostNewContract([FromBody] CreateContractRequestObject createContractRequestObject)
        {
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(HttpContext));

            var contract = await _postNewContractUseCase.ExecuteAsync(createContractRequestObject, token).ConfigureAwait(false);
            return Created(new Uri($"api/v1/contracts/{contract.Id}", UriKind.Relative), contract);
        }
    }
}
