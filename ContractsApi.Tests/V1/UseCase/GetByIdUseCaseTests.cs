using ContractsApi.V1.Gateways;
using ContractsApi.V1.UseCase;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;

namespace ContractsApi.Tests.V1.UseCase
{
    public class GetByIdUseCaseTests : LogCallAspectFixture
    {
        private Mock<IContractGateway> _mockGateway;
        private GetContractByIdUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IContractGateway>();
            _classUnderTest = new GetContractByIdUseCase(_mockGateway.Object);
        }

        //TODO: test to check that the use case retrieves the correct record from the database.
        //Guidance on unit testing and example of mocking can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Unit-Tests
    }
}
