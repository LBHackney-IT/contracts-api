using System;
using System.Threading.Tasks;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Controllers;
using ContractsApi.V1.Domain;
using ContractsApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Testing.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ContractsApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class ContractsApiControllerTests
    {
        private readonly ContractsApiController _classUnderTest;
        private readonly Mock<IGetContractByIdUseCase> _mockGetByIdUseCase;
        private readonly Mock<IPostNewContractUseCase> _mockPostNewContractUseCase;
        private readonly Mock<ITokenFactory> _mockTokenFactory;
        private readonly Mock<IHttpContextWrapper> _mockContextWrapper;

        public ContractsApiControllerTests()
        {
            _mockGetByIdUseCase = new Mock<IGetContractByIdUseCase>();
            _mockPostNewContractUseCase = new Mock<IPostNewContractUseCase>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _classUnderTest = new ContractsApiController(_mockGetByIdUseCase.Object, _mockPostNewContractUseCase.Object,
                _mockTokenFactory.Object, _mockContextWrapper.Object);
        }

        private ContractQueryRequest ConstructRequest(Guid? id = null)
        {
            return new ContractQueryRequest() { Id = id ?? Guid.NewGuid() };
        }

        [Fact]
        public async Task GetContractByIdUseCaseReturns404IfNoIfNoContractFound()
        {
            var request = ConstructRequest();
            _mockGetByIdUseCase.Setup(x => x.Execute(request)).ReturnsAsync((ContractResponseObject) null);

            var response = await _classUnderTest.GetContractById(request).ConfigureAwait(false);

            response.Should().BeOfType(typeof(OkObjectResult));
           (response as NotFoundObjectResult).Value.Should().Be(request.Id);
        }
    }
}
