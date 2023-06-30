using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Controllers;
using ContractsApi.V1.Infrastructure;
using ContractsApi.V1.UseCase.Interfaces;
using ContractsApi.Tests.V1.Helper;
using FluentAssertions;
using Hackney.Core.DynamoDb;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;



namespace ContractsApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class ContractsApiControllerTests
    {
        private readonly ContractsApiController _classUnderTest;
        private readonly Mock<IGetContractByIdUseCase> _mockGetByIdUseCase;
        private readonly Mock<IGetContractsByTargetIdUseCase> _mockGetContractsByTargetIdUseCase;
        private readonly Mock<IPostNewContractUseCase> _mockPostNewContractUseCase;
        private Mock<IPatchContractUseCase> _mockPatchContractUseCase;
        private readonly Mock<ITokenFactory> _mockTokenFactory;
        private readonly Mock<IHttpContextWrapper> _mockContextWrapper;
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<HttpRequest> _mockHttpRequest;
        private readonly HeaderDictionary _requestHeaders;
        private readonly Mock<HttpResponse> _mockHttpResponse;
        private readonly HeaderDictionary _responseHeaders;

        private const string RequestBodyText = "Some request body text";

        public ContractsApiControllerTests()
        {
            _mockGetByIdUseCase = new Mock<IGetContractByIdUseCase>();
            _mockGetContractsByTargetIdUseCase = new Mock<IGetContractsByTargetIdUseCase>();
            _mockPostNewContractUseCase = new Mock<IPostNewContractUseCase>();
            _mockPatchContractUseCase = new Mock<IPatchContractUseCase>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _classUnderTest = new ContractsApiController(_mockGetByIdUseCase.Object, _mockGetContractsByTargetIdUseCase.Object, _mockPostNewContractUseCase.Object, _mockPatchContractUseCase.Object,
                _mockTokenFactory.Object, _mockContextWrapper.Object);

            _mockHttpRequest = new Mock<HttpRequest>();
            _mockHttpResponse = new Mock<HttpResponse>();

            _mockHttpRequest.SetupGet(x => x.Body)
                .Returns(new MemoryStream(Encoding.Default.GetBytes(RequestBodyText)));

            _requestHeaders = new HeaderDictionary();
            _mockHttpRequest.SetupGet(x => x.Headers).Returns(_requestHeaders);

            _mockContextWrapper
                .Setup(x => x.GetContextRequestHeaders(It.IsAny<HttpContext>()))
                .Returns(_requestHeaders);

            _responseHeaders = new HeaderDictionary();
            _mockHttpResponse.SetupGet(x => x.Headers).Returns(_responseHeaders);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);

            var controllerContext = new ControllerContext(new ActionContext(mockHttpContext.Object, new RouteData(),
                new ControllerActionDescriptor()));
            _classUnderTest.ControllerContext = controllerContext;
        }

        [Fact]
        public async Task GetContractByIdUseCaseReturns404IfNoIfNoContractFound()
        {
            var request = BoundaryHelper.ConstructRequest();
            _mockGetByIdUseCase.Setup(x => x.Execute(request)).ReturnsAsync((ContractResponseObject) null);

            var response = await _classUnderTest.GetContractById(request).ConfigureAwait(false);

            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(request.Id);
        }

        [Fact]
        public async Task GetContractByIdUseCaseReturnsAContract()
        {
            var foundContract = _fixture.Create<ContractResponseObject>();
            var request = BoundaryHelper.ConstructRequest(foundContract.Id);
            _mockGetByIdUseCase.Setup(x => x.Execute(request)).ReturnsAsync(foundContract);

            var response = await _classUnderTest.GetContractById(request).ConfigureAwait(false);

            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().Be(foundContract);

            var expectedEtagValue = $"\"{foundContract.VersionNumber}\"";
            _classUnderTest.HttpContext.Response.Headers.TryGetValue(HeaderConstants.ETag, out StringValues val)
                .Should().BeTrue();
            val.First().Should().Be(expectedEtagValue);
        }

        [Fact]
        public async Task GetContractWhenVersionNumberIsNullReturnsEmptyETag()
        {
            var mockContractResponse = _fixture.Build<ContractResponseObject>()
                .With(x => x.VersionNumber, (int?) null)
                .Create();

            var mockRequest = BoundaryHelper.ConstructRequest(mockContractResponse.Id);

            _mockGetByIdUseCase.Setup(x => x.Execute(mockRequest)).ReturnsAsync(mockContractResponse);

            await _classUnderTest.GetContractById(mockRequest).ConfigureAwait(false);

            var expectedEtagValue = $"\"\"";
            _classUnderTest.HttpContext.Response.Headers.TryGetValue(HeaderConstants.ETag, out StringValues val)
                .Should().BeTrue();
            val.First().Should().Be(expectedEtagValue);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task GetContractsByTargetIdReturnsContractsForATargetId(string paginationToken)
        {
            var id = Guid.NewGuid();
            var request = new GetContractsQueryRequest { TargetId = id, PaginationToken = paginationToken };
            var contracts = _fixture.CreateMany<ContractResponseObject>(5).ToList();
            var pagedResult = new PagedResult<ContractResponseObject>(contracts, new PaginationDetails(paginationToken));

            _mockGetContractsByTargetIdUseCase.Setup(x => x.Execute(request)).ReturnsAsync(pagedResult);

            var response = await _classUnderTest.GetContractsByTargetId(request).ConfigureAwait(false);

            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(pagedResult);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task GetContractsByTargetIdReturns200OkWithNoResults(string paginationToken)
        {
            var id = Guid.NewGuid();
            var request = new GetContractsQueryRequest { TargetId = id, PaginationToken = paginationToken };
            var result = _fixture.Build<PagedResult<ContractResponseObject>>()
                .With(x => x.Results, new List<ContractResponseObject>()).Create();

            _mockGetContractsByTargetIdUseCase.Setup(x => x.Execute(request)).ReturnsAsync(result);

            var response = await _classUnderTest.GetContractsByTargetId(request).ConfigureAwait(false);

            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task PostNewContractReturns201CreatedResponseWhenSuccessful()
        {
            var newContract = _fixture.Create<ContractResponseObject>();
            var request = BoundaryHelper.ConstructPostRequest();
            _mockPostNewContractUseCase.Setup(x => x.ExecuteAsync(request, It.IsAny<Token>())).ReturnsAsync(newContract);

            var response = await _classUnderTest.PostNewContract(request).ConfigureAwait(false);

            response.Should().BeOfType(typeof(CreatedResult));
            (response as CreatedResult).Value.Should().Be(newContract);
        }

        [Fact]
        public async Task PatchContractReturns204NoContentIfSuccessful()
        {
            var mockGuid = new Guid();
            var mockRequestObject = _fixture.Create<EditContractRequest>();

            _mockPatchContractUseCase
                .Setup(x => x.ExecuteAsync(mockGuid, mockRequestObject, It.IsAny<string>(), It.IsAny<Token>(),
                    It.IsAny<int?>())).ReturnsAsync(_fixture.Create<ContractResponseObject>());

            var response = await _classUnderTest.PatchContract(mockGuid, mockRequestObject).ConfigureAwait(false);

            response.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async Task PatchContractReturns404NotFoundIfNoContractIsFound()
        {
            var mockGuid = new Guid();
            var mockRequestObject = _fixture.Create<EditContractRequest>();

            _mockPatchContractUseCase
                .Setup(x => x.ExecuteAsync(mockGuid, mockRequestObject, It.IsAny<string>(), It.IsAny<Token>(),
                    It.IsAny<int?>())).ReturnsAsync((ContractResponseObject) null);

            var response = await _classUnderTest.PatchContract(mockGuid, mockRequestObject).ConfigureAwait(false);

            response.Should().BeOfType(typeof(NotFoundResult));
        }

       [Fact]
        public async Task PatchContractReturnsErrorIfValidationFails()
        {
            var mockGuid = new Guid();
            var mockRequestObject = _fixture.Create<EditContractRequest>();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            mockRequestObject.StartDate = tomorrow;
            mockRequestObject.HandbackDate = today;

            _mockPatchContractUseCase
                .Setup(x => x.ExecuteAsync(mockGuid, mockRequestObject, It.IsAny<string>(), It.IsAny<Token>(),
                    It.IsAny<int?>())).ReturnsAsync((ContractResponseObject) null);

            var response = await _classUnderTest.PatchContract(mockGuid, mockRequestObject).ConfigureAwait(false);
            var statusCode = GetStatusCode(response);
            statusCode.Should().Be(400);
        }
 
        protected static int? GetStatusCode(IActionResult result)
        {
            return (result as IStatusCodeActionResult).StatusCode;
        }

    }
}
