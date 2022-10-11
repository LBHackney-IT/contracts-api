using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Gateways;
using ContractsApi.V1.Infrastructure;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractsApi.Tests.V1.Helper;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Infrastructure.Exceptions;
using Force.DeepCloner;
using Hackney.Core.JWT;
using Hackney.Core.Testing.DynamoDb;
using Xunit;

namespace ContractsApi.Tests.V1.Gateways
{

    [Collection("AppTest collection")]
    public class DynamoDbGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly DynamoDbGateway _classUnderTest;
        private readonly Mock<ILogger<DynamoDbGateway>> _logger;
        private readonly IDynamoDbFixture _dbFixture;
        private readonly Mock<IEntityUpdater> _updater;


        public DynamoDbGatewayTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _logger = new Mock<ILogger<DynamoDbGateway>>();
            _updater = new Mock<IEntityUpdater>();
            _classUnderTest = new DynamoDbGateway(_dbFixture.DynamoDbContext, _logger.Object, _updater.Object);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }

        private async Task InsertDataIntoDynamoDB(ContractDb entity)
        {
            await _dbFixture.SaveEntityAsync(entity).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetContractByIdReturnsNullIfNoContractFound()
        {
            var request = BoundaryHelper.ConstructRequest();
            var response = await _classUnderTest.GetContractById(request).ConfigureAwait(false);

            response.Should().BeNull();
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {request.Id}", Times.Once());
        }

        [Fact]
        public async Task GetContractByIdReturnsContractIfItExists()
        {
            var contract = _fixture.Build<ContractDb>()
                .With(x => x.VersionNumber, (int?) null)
                .Create();

            await InsertDataIntoDynamoDB(contract).ConfigureAwait(false);

            var request = BoundaryHelper.ConstructRequest(contract.Id);
            var response = await _classUnderTest.GetContractById(request).ConfigureAwait(false);

            response.Should().BeEquivalentTo(contract);
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {request.Id}", Times.Once());
        }

        [Fact]
        public async Task PostNewContractAsyncAddsContractToDatabase()
        {
            var contract = _fixture.Build<ContractDb>()
                .With(x => x.VersionNumber, (int?) null)
                .Create();

            await InsertDataIntoDynamoDB(contract).ConfigureAwait(false);
            var response = await _classUnderTest.PostNewContractAsync(contract).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(contract.ToDomain());

            await _dbFixture.DynamoDbContext.DeleteAsync<ContractDb>(contract.Id).ConfigureAwait(false);
        }

        [Fact]
        public async Task PatchContractReturnsNullIfContractNotFound()
        {
            var mockGuid = new Guid();
            var mockRequestObject = _fixture.Create<EditContractRequest>();
            var mockRawBody = "";

            var response = await _classUnderTest
                .PatchContract(mockGuid, mockRequestObject, mockRawBody, It.IsAny<int?>())
                .ConfigureAwait(false);

            response.Should().BeNull();
        }

        [Fact]
        public async Task PatchContractThrowsConflictVersionError()
        {
            var currentContract = _fixture.Build<ContractDb>().With(x => x.VersionNumber, (int?) null).Create();

            await InsertDataIntoDynamoDB(currentContract).ConfigureAwait(false);

            var contractId = currentContract.Id;
            var request = _fixture.Create<EditContractRequest>();
            var suppliedVersion = 1;

            Func<Task<UpdateEntityResult<ContractDb>>> func = async () =>
                await _classUnderTest.PatchContract(contractId, request, It.IsAny<string>(), suppliedVersion)
                    .ConfigureAwait(false);

            func.Should().Throw<VersionNumberConflictException>().WithMessage($"The version number supplied ({suppliedVersion}) does not match the current value on the entity ({0}).");
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync to update id {contractId}", Times.Never());
        }

        [Fact]
        public async Task PatchContractSuccessfullyUpdatesAContract()
        {
            var currentContract = _fixture.Build<ContractDb>().With(x => x.VersionNumber, (int?) null).Create();

            await InsertDataIntoDynamoDB(currentContract).ConfigureAwait(false);

            var contractId = currentContract.Id;
            var request = _fixture.Create<EditContractRequest>();
            var requestBody = "{ \"StartDate\":\"key7d2d6e42-0cbf-411a-b66c-bc35da8b6061\":{ },\"EndDate\":\"89017f11-95f7-434d-96f8-178e33685fb4\"}}";
            var suppliedVersion = 0;

            var updatedContract = currentContract.DeepClone();
            updatedContract.StartDate = request.StartDate;
            updatedContract.EndDate = request.EndDate;
            updatedContract.VersionNumber = suppliedVersion;

            _updater.Setup(x => x.UpdateEntity(It.IsAny<ContractDb>(), requestBody, request)).Returns(
                new UpdateEntityResult<ContractDb>()
                {
                    UpdatedEntity = updatedContract,
                    OldValues = new Dictionary<string, object>
                    {
                        { "startDate", currentContract.StartDate }, { "endDate", currentContract.EndDate }
                    },
                    NewValues = new Dictionary<string, object>
                    {
                        { "startDate", updatedContract.StartDate }, { "endDate", updatedContract.EndDate }
                    }
                });


            var response = await _classUnderTest.PatchContract(contractId, request, requestBody, suppliedVersion).ConfigureAwait(false);

            var load = await _dbFixture.DynamoDbContext.LoadAsync<ContractDb>(contractId).ConfigureAwait(false);

            load.StartDate.Should().Be(request.StartDate);
            load.EndDate.Should().Be(request.EndDate);

            await _dbFixture.DynamoDbContext.DeleteAsync<ContractDb>(contractId).ConfigureAwait(false);
        }
    }
}
