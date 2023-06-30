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
using ContractsApi.V1.Infrastructure.Exceptions;
using Hackney.Core.DynamoDb;
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
        private readonly IGetContractsByTargetIdUseCase _getContractsByTargetIdUseCase;
        private readonly IPostNewContractUseCase _postNewContractUseCase;
        private readonly IPatchContractUseCase _patchContractUseCase;
        private readonly ITokenFactory _tokenFactory;
        private readonly IHttpContextWrapper _contextWrapper;

        public ContractsApiController(IGetContractByIdUseCase getContractByIdUseCase, IGetContractsByTargetIdUseCase getContractsByTargetIdUseCase,
            IPostNewContractUseCase postNewContractUseCase, IPatchContractUseCase patchContractUseCase, ITokenFactory tokenFactory,
            IHttpContextWrapper contextWrapper)
        {
            _getContractByIdUseCase = getContractByIdUseCase;
            _getContractsByTargetIdUseCase = getContractsByTargetIdUseCase;
            _postNewContractUseCase = postNewContractUseCase;
            _patchContractUseCase = patchContractUseCase;

            _tokenFactory = tokenFactory;
            _contextWrapper = contextWrapper;
        }

        /// <summary>
        /// Retrieves the contract with the supplied id
        /// </summary>
        /// <response code="200">Successfully retrieved details for the specified ID</response>
        /// <response code="404">No contract information found for the specified ID</response>
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
        /// <summary>
        /// Retrieves assets with the supplied target id
        /// </summary>
        /// <response code="200">Successfully retrieved details for the specified target ID</response>
        /// <response code="404">No contracts found for the specified target ID</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(PagedResult<ContractResponseObject>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> GetContractsByTargetId([FromQuery] GetContractsQueryRequest query)
        {
            var result = await _getContractsByTargetIdUseCase.Execute(query).ConfigureAwait(false);

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

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        [Route("{id}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> PatchContract([FromRoute] Guid id, [FromBody] EditContractRequest contract)
        {
            var bodyText = await HttpContext.Request.GetRawBodyStringAsync().ConfigureAwait(false);
            var ifMatch = GetIfMatchFromHeader();
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(HttpContext));
            var validator = new CustomEditContractValidation();
            var validationResults = validator.Validate(contract);

            if (!validationResults.IsValid)
            {
                return BadRequest(validationResults.ToString());
            }

            try
            {
                var result = await _patchContractUseCase.ExecuteAsync(id, contract, bodyText, token, ifMatch)
                    .ConfigureAwait(false);

                if (result == null) return NotFound();

                return NoContent();
            }
            catch (VersionNumberConflictException e)
            {
                return Conflict(e.Message);
            }
        }

        private int? GetIfMatchFromHeader()
        {
            var header = HttpContext.Request.Headers.GetHeaderValue(HeaderConstants.IfMatch);

            if (header == null)
                return null;

            _ = EntityTagHeaderValue.TryParse(header, out var entityTagHeaderValue);

            if (entityTagHeaderValue == null)
                return null;

            var version = entityTagHeaderValue.Tag.Replace("\"", string.Empty);

            if (int.TryParse(version, out var numericValue))
                return numericValue;

            return null;
        }
    }
}
