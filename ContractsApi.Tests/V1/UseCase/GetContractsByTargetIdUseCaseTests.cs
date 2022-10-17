using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Gateways;
using ContractsApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.DynamoDb;
using Moq;
using Xunit;

namespace ContractsApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetContractsByTargetIdUseCaseTests
    {
        private readonly Mock<IContractGateway> _mockGateway;
        private readonly GetContractsByTargetIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public GetContractsByTargetIdUseCaseTests()
        {
            _mockGateway = new Mock<IContractGateway>();
            _classUnderTest = new GetContractsByTargetIdUseCase(_mockGateway.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task UseCaseReturnsAnEmptyListIfNoContractsFound(string paginationToken)
        {
            var id = Guid.NewGuid();
            var request = new GetContractsQueryRequest {TargetId = id, PaginationToken = paginationToken};
            var result = new PagedResult<Contract>(new List<Contract>(), new PaginationDetails(paginationToken));
            _mockGateway.Setup(x => x.GetContractsByTargetId(request)).ReturnsAsync(result);

            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            response.Results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task UseCaseReturnsAnEmptyListIfGatewayReturnsNull(string paginationToken)
        {
            var id = Guid.NewGuid();
            var request = new GetContractsQueryRequest {TargetId = id, PaginationToken = paginationToken};
            var result = new PagedResult<Contract>(null, new PaginationDetails(paginationToken));
            _mockGateway.Setup(x => x.GetContractsByTargetId(request)).ReturnsAsync(result);

            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            response.Results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task UseCaseReturnsAListOfContracts(string paginationToken)
        {
            var id = Guid.NewGuid();
            var request = new GetContractsQueryRequest {TargetId = id, PaginationToken = paginationToken};
            var contracts = _fixture.CreateMany<Contract>(5).ToList();
            var result = new PagedResult<Contract>(contracts, new PaginationDetails(paginationToken));
            _mockGateway.Setup(x => x.GetContractsByTargetId(request)).ReturnsAsync(result);

            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            response.Results.Should().BeEquivalentTo(contracts.ToResponse());
        }
    }
}
