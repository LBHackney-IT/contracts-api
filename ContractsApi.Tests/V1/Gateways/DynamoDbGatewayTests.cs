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
using System.Threading.Tasks;
using ContractsApi.Tests.V1.Helper;
using ContractsApi.V1.Factories;
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


        public DynamoDbGatewayTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _logger = new Mock<ILogger<DynamoDbGateway>>();
            _classUnderTest = new DynamoDbGateway(_dbFixture.DynamoDbContext, _logger.Object);
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
    }
}
