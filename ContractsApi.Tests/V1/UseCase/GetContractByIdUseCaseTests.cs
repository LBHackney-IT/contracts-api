using System.Threading.Tasks;
using AutoFixture;
using ContractsApi.Tests.V1.Helper;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Gateways;
using ContractsApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using Xunit;

namespace ContractsApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetContractByIdUseCaseTests
    {
        private readonly Mock<IContractGateway> _mockGateway;
        private readonly GetContractByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public GetContractByIdUseCaseTests()
        {
            _mockGateway = new Mock<IContractGateway>();
            _classUnderTest = new GetContractByIdUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task UseCaseReturnsNullWhenNoContractFound()
        {
            var request = BoundaryHelper.ConstructRequest();
            _mockGateway.Setup(x => x.GetContractById(request)).ReturnsAsync((Contract) null);

            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);
            response.Should().BeNull();
        }

        [Fact]
        public async Task UseCaseReturnsContractWhenContractIsFound()
        {
            var contract = _fixture.Create<Contract>();
            var request = BoundaryHelper.ConstructRequest(contract.Id);
            _mockGateway.Setup(x => x.GetContractById(request)).ReturnsAsync(contract);

            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);
            response.Should().BeEquivalentTo(contract.ToResponse());
        }
    }
}
