using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
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
    public class PatchContractUseCaseTests
    {
        private readonly Mock<IContractGateway> _mockGateway;
        private readonly PatchContractUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ISnsGateway> _contractSnsGateway;
        private readonly Mock<ISnsFactory> _contractSnsFactory;

        public PatchContractUseCaseTests()
        {
            _mockGateway = new Mock<IContractGateway>();
            _contractSnsGateway = new Mock<ISnsGateway>();
            _contractSnsFactory = new Mock<ISnsFactory>();
            _classUnderTest = new PatchContractUseCase(_mockGateway.Object, _contractSnsGateway.Object, _contractSnsFactory.Object);
        }

        [Fact]
        public async Task UseCaseReturnsNullIfNoContractIsFound()
        {
            var mockGuid = new Guid();
            var mockRequestObject = _fixture.Create<EditContractRequest>();
            var mockRawBody = "";
            var mockToken = _fixture.Create<Token>();
            _mockGateway.Setup(x => x.PatchContract(mockGuid, mockRequestObject, It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync((UpdateEntityResult<ContractDb>) null);

            var response = await _classUnderTest.ExecuteAsync(mockGuid, mockRequestObject, mockRawBody, mockToken, null).ConfigureAwait(false);

            response.Should().BeNull();
        }

        [Fact]
        public async Task UseCaseReturnsContractResponseObjectIfSuccessful()
        {
            var mockGuid = new Guid();
            var mockRequestObject = _fixture.Create<EditContractRequest>();
            var mockRawBody = "";
            var mockToken = _fixture.Create<Token>();
            var gatewayResponse = new UpdateEntityResult<ContractDb>
            {
                UpdatedEntity = _fixture.Create<ContractDb>()
            };
            _mockGateway.Setup(x => x.PatchContract(mockGuid, mockRequestObject, It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(gatewayResponse);

            var response = await _classUnderTest.ExecuteAsync(mockGuid, mockRequestObject, mockRawBody, mockToken, null).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ContractResponseObject));

            response.RelatedPeople.Should().BeEquivalentTo(gatewayResponse.UpdatedEntity.RelatedPeople);
        }

        [Fact]
        public async Task UseCaseDoesNotCallSnsGatewayIfContractIsntEdited()
        {
            var mockGuid = new Guid();
            var mockRequestObject = _fixture.Create<EditContractRequest>();
            var mockRawBody = "";
            var mockToken = _fixture.Create<Token>();
            var gatewayResponse = new UpdateEntityResult<ContractDb>
            {
                UpdatedEntity = _fixture.Create<ContractDb>()
            };

            _mockGateway.Setup(x => x.PatchContract(mockGuid, mockRequestObject, It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(gatewayResponse);

            var response = await _classUnderTest.ExecuteAsync(mockGuid, mockRequestObject, mockRawBody, mockToken, null).ConfigureAwait(false);

            response.Should().BeOfType(typeof(ContractResponseObject));

            _contractSnsGateway.Verify(x => x.Publish(It.IsAny<EntityEventSns>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UseCaseCallsSnsGatewayWhenEditIsSuccessful()
        {
            var mockGuid = new Guid();
            var mockRequestObject = _fixture.Create<EditContractRequest>();
            var mockRawBody = "";
            var mockToken = _fixture.Create<Token>();
            var gatewayResponse = new UpdateEntityResult<ContractDb>
            {
                UpdatedEntity = _fixture.Create<ContractDb>(),
                NewValues = new Dictionary<string, object>
                {
                    {"startDate", _fixture.Create<DateTime>()}
                }
            };

            _mockGateway.Setup(x => x.PatchContract(mockGuid, mockRequestObject, It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(gatewayResponse);

            var snsEvent = _fixture.Create<EntityEventSns>();

            _contractSnsFactory.Setup(x => x.UpdateContract(gatewayResponse, mockToken)).Returns(snsEvent);

            var response = await _classUnderTest.ExecuteAsync(mockGuid, mockRequestObject, mockRawBody, mockToken, null).ConfigureAwait(false);

            response.Should().BeOfType(typeof(ContractResponseObject));

            _contractSnsGateway.Verify(x => x.Publish(snsEvent, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
