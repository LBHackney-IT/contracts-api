using System;
using System.Threading.Tasks;
using AutoFixture;
using ContractsApi.Tests.V1.Helper;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Gateways;
using ContractsApi.V1.Infrastructure;
using ContractsApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Moq;
using Xunit;

namespace ContractsApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class PostNewContractUseCaseTests
    {
        private readonly Mock<IContractGateway> _mockGateway;
        private readonly PostNewContractUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ISnsGateway> _contractSnsGateway;
        private readonly ContractSnsFactory _contractSnsFactory;

        public PostNewContractUseCaseTests()
        {
            _mockGateway = new Mock<IContractGateway>();
            _contractSnsGateway = new Mock<ISnsGateway>();
            _contractSnsFactory = new ContractSnsFactory();
            _classUnderTest = new PostNewContractUseCase(_mockGateway.Object, _contractSnsGateway.Object, _contractSnsFactory);
        }

        [Fact]
        public async Task UseCaseShouldReturnContractIfSuccessfullyCreated()
        {
            var contract = _fixture.Create<Contract>();
            var token = new Token();
            var request = BoundaryHelper.ConstructPostRequest();
            _mockGateway.Setup(x => x.PostNewContractAsync(It.IsAny<ContractDb>())).ReturnsAsync(contract);

            var response = await _classUnderTest.ExecuteAsync(request, token).ConfigureAwait(false);

            response.Should().BeEquivalentTo(contract.ToResponse());
        }
    }
}
