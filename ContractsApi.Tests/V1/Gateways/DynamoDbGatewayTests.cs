using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Infrastructure;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Moq;
using Microsoft.Extensions.Logging;
using ContractsApi.V1.Gateways;
using Hackney.Core.Testing.Shared;
using System.Linq;
using ContractsApi.Tests.V1.Helper;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Infrastructure.Exceptions;
using Force.DeepCloner;
using Hackney.Core.JWT;
using Hackney.Core.Testing.DynamoDb;

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

        private List<ContractDb> UpsertContracts(Guid targetId, int count)
        {
            var contracts = new List<ContractDb>();

            contracts.AddRange(_fixture.Build<ContractDb>().With(x => x.TargetId, targetId).With(x => x.TargetType, "asset").With(x => x.VersionNumber, (int?) null).With(x => x.HandbackDate, (DateTime?) null).With(x => x.ApprovalStatus, ApprovalStatus.PendingApproval).CreateMany(count));

            foreach (var contract in contracts)
            {
                _dbFixture.SaveEntityAsync(contract).GetAwaiter().GetResult();
            }

            return contracts;
        }

        [Fact]
        public async Task GetContractByIdReturnsNullIfNoContractFound()
        {
            var request = BoundaryHelper.ConstructRequest();
            var response = await _classUnderTest.GetContractById(request).ConfigureAwait(false);

            response.Should().BeNull();
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {request.Id}",
                Times.Once());
        }

        [Fact]
        public async Task GetContractByIdReturnsContractIfItExists()
        {
            var contract = _fixture.Build<ContractDb>().With(x => x.VersionNumber, (int?) null).Create();
            await InsertDataIntoDynamoDB(contract).ConfigureAwait(false);
            var request = BoundaryHelper.ConstructRequest(contract.Id);
            var response = await _classUnderTest.GetContractById(request).ConfigureAwait(false);
            response.Should().BeEquivalentTo(contract);
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {request.Id}",
                Times.Once());
        }

        [Fact]
        public async Task GetContractsByTargetIdReturnsEmptyIfNoRecords()
        {
            var id = Guid.NewGuid();
            var request = new GetContractsQueryRequest { TargetId = id, TargetType = "asset" };
            var response = await _classUnderTest.GetContractsByTargetId(request).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Results.Should().BeEmpty();
            response.PaginationDetails.HasNext.Should().BeFalse();
            response.PaginationDetails.NextToken.Should().BeNull();
        }

        [Fact]
        public async Task GetContractsByTargetIdReturnsContractsIfFound()
        {
            var id = Guid.NewGuid();
            var request = new GetContractsQueryRequest { TargetId = id, TargetType = "asset" };
            var contracts = UpsertContracts(id, 5);
            var response = await _classUnderTest.GetContractsByTargetId(request).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Results.Should().BeEquivalentTo(contracts);
            response.PaginationDetails.HasNext.Should().BeFalse();
            response.PaginationDetails.NextToken.Should().BeNull();
        }

        [Fact]
        public async Task GetContractsByTargetIdReturnsMultiplePagesOfRecords()
        {
            var id = Guid.NewGuid();
            var request = new GetContractsQueryRequest { TargetId = id, TargetType = "asset", PageSize = 5 };
            var contracts = UpsertContracts(id, 9);
            var expectedResults = contracts.OrderByDescending(x => x.Id);

            //Dynamo does not bring back results in any order so for this I'd join both results then check
            var result = await _classUnderTest.GetContractsByTargetId(request).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.PaginationDetails.HasNext.Should().BeTrue();
            result.PaginationDetails.NextToken.Should().NotBeNull();

            request.PaginationToken = result.PaginationDetails.NextToken;
            var result2 = await _classUnderTest.GetContractsByTargetId(request).ConfigureAwait(false);
            result2.Should().NotBeNull();

            var allRecordsOrdered = result.Results;
            allRecordsOrdered.AddRange(result2.Results);
            allRecordsOrdered.OrderByDescending(x => x.Id).Should().BeEquivalentTo(expectedResults);

            result2.PaginationDetails.HasNext.Should().BeFalse();
            result2.PaginationDetails.NextToken.Should().BeNull();
        }

        [Fact]
        public async Task GetContractsByTargetIdReturnsNoPaginationTokenIfPageSizeEqualsRecordCount()
        {
            var id = Guid.NewGuid();
            var request = new GetContractsQueryRequest { TargetId = id, TargetType = "asset", PageSize = 10 };
            var contracts = UpsertContracts(id, 10);

            var response = await _classUnderTest.GetContractsByTargetId(request).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Results.Should().BeEquivalentTo(contracts);
            response.PaginationDetails.HasNext.Should().BeFalse();
            response.PaginationDetails.NextToken.Should().BeNull();
        }

        [Fact]
        public async Task PostNewContractAsyncAddsContractToDatabase()
        {
            var contract = _fixture.Build<ContractDb>().With(x => x.VersionNumber, (int?) null).Create();
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
            var response = await _classUnderTest.PatchContract(mockGuid, mockRequestObject, mockRawBody, It.IsAny<int?>()).ConfigureAwait(false);
            response.Should().BeNull();
        }

        [Fact]
        public async Task PatchContractThrowsDatesError()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var currentContract = _fixture.Build<ContractDb>().With(x => x.StartDate, (DateTime?) tomorrow).With(x => x.VersionNumber, (int?) null).Create();
            await InsertDataIntoDynamoDB(currentContract).ConfigureAwait(false);
            var contractId = currentContract.Id;
            var request = _fixture.Create<EditContractRequest>();
            request.HandbackDate = today;

            Func<Task<UpdateEntityResult<ContractDb>>> func = async () =>
                await _classUnderTest.PatchContract(contractId, request, It.IsAny<string>(), It.IsAny<int>())
                    .ConfigureAwait(false);

            await func.Should().ThrowAsync<StartAndHandbackDatesConflictException>().WithMessage(
                $"Handback date ({request.HandbackDate}) cannot be prior to Start date ({currentContract.StartDate}).");
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync to update id {contractId}",
                Times.Never());
        }

        [Fact]
        public async Task PatchContractThrowsDatesErrorWhenStartDateIsNull()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var currentContract = _fixture.Build<ContractDb>().With(x => x.StartDate, (DateTime?) null).With(x => x.VersionNumber, (int?) null).Create();

            await InsertDataIntoDynamoDB(currentContract).ConfigureAwait(false);

            var contractId = currentContract.Id;
            var request = _fixture.Create<EditContractRequest>();
            request.HandbackDate = today;

            Func<Task<UpdateEntityResult<ContractDb>>> func = async () =>
                await _classUnderTest.PatchContract(contractId, request, It.IsAny<string>(), It.IsAny<int>()).ConfigureAwait(false);

            await func.Should().ThrowAsync<StartAndHandbackDatesConflictException>().WithMessage($"Handback date ({request.HandbackDate}) cannot be prior to Start date ({{null}}).");
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync to update id {contractId}",
                Times.Never());
        }


        [Fact]
        public async Task PatchContractThrowsConflictVersionError()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var currentContract = _fixture.Build<ContractDb>().With(x => x.VersionNumber, (int?) null).With(x => x.StartDate, (DateTime?) today).Create();

            await InsertDataIntoDynamoDB(currentContract).ConfigureAwait(false);

            var contractId = currentContract.Id;
            var request = _fixture.Create<EditContractRequest>();
            var suppliedVersion = 1;
            request.HandbackDate = tomorrow;

            Func<Task<UpdateEntityResult<ContractDb>>> func = async () =>
                await _classUnderTest.PatchContract(contractId, request, It.IsAny<string>(), suppliedVersion).ConfigureAwait(false);

            await func.Should().ThrowAsync<VersionNumberConflictException>().WithMessage($"The version number supplied ({suppliedVersion}) does not match the current value on the entity ({0}).");
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync to update id {contractId}",
                Times.Never());
        }

        [Fact]
        public async Task PatchContractThrowsErrorIfTryingToSuspendBlock()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var currentContract = _fixture.Build<ContractDb>().With(x => x.VersionNumber, (int?) null).With(x => x.StartDate, (DateTime?) today).Create();

            await InsertDataIntoDynamoDB(currentContract).ConfigureAwait(false);

            var contractId = currentContract.Id;
            var request = _fixture.Create<EditContractRequest>();
            var suppliedVersion = 0;
            request.HandbackDate = tomorrow;
            request.SuspensionDate = tomorrow;
            request.ContractManagement.AssetHierarchy = AssetHierarchy.Block;

            Func<Task<UpdateEntityResult<ContractDb>>> func = async () =>
                await _classUnderTest.PatchContract(contractId, request, It.IsAny<string>(), suppliedVersion).ConfigureAwait(false);

            await func.Should().ThrowAsync<SuspendingBlockException>().WithMessage($"It is not possible to add a suspension to blocks");
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync to update id {contractId}",
                Times.Never());
        }

        [Fact]
        public async Task PatchContractSuccessfullyUpdatesAContract()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var currentContract = _fixture.Build<ContractDb>().With(x => x.VersionNumber, (int?) null).With(x => x.StartDate, (DateTime?) today).Create();

            await InsertDataIntoDynamoDB(currentContract).ConfigureAwait(false);

            var contractId = currentContract.Id;
            var request = _fixture.Create<EditContractRequest>();
            request.HandbackDate = tomorrow;
            request.SuspensionDate = null;
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

        [Fact]
        public async Task ApprovalStatusStoredAndRetrievedCorrectly()
        {
            // Arrange
            var contract = _fixture.Build<ContractDb>().With(x => x.VersionNumber, (int?) null).With(x => x.ApprovalStatus, ApprovalStatus.PendingApproval).Create();
            // Act
            await _classUnderTest.PostNewContractAsync(contract).ConfigureAwait(false);
            // Assert
            var storedContract = await _dbFixture.DynamoDbContext.LoadAsync<ContractDb>(contract.Id).ConfigureAwait(false);
            storedContract.Should().NotBeNull();
            storedContract.ApprovalStatus.Should().Be(ApprovalStatus.PendingApproval);
            // Cleanup
            await _dbFixture.DynamoDbContext.DeleteAsync<ContractDb>(contract.Id).ConfigureAwait(false);
        }

        [Fact]
        public void ApprovalStatusStoredAsStringInDynamoDb()
        {
            // Arrange
            var contract = _fixture.Build<ContractDb>().With(x => x.VersionNumber, (int?) null).With(x => x.ApprovalStatus, ApprovalStatus.PendingApproval).Create();
            //Act
            var document = _dbFixture.DynamoDbContext.ToDocument(contract);
            //Assert
            var approvalStatusAttribute = document["approvalStatus"];
            approvalStatusAttribute.Should().NotBeNull();
            approvalStatusAttribute.ToString().Should().Be("PendingApproval");
        }
    }
}
