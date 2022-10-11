using System;
using Amazon.DynamoDBv2.DataModel;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Infrastructure;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using ContractsApi.V1.Infrastructure.Exceptions;
using Hackney.Core.JWT;

namespace ContractsApi.V1.Gateways
{
    public class DynamoDbGateway : IContractGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<DynamoDbGateway> _logger;
        private readonly IEntityUpdater _updater;


        public DynamoDbGateway(IDynamoDBContext dynamoDbContext, ILogger<DynamoDbGateway> logger, IEntityUpdater updater)
        {
            _dynamoDbContext = dynamoDbContext;
            _logger = logger;
            _updater = updater;
        }

        [LogCall]
        public async Task<Contract> GetContractById(ContractQueryRequest query)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id}");

            var result = await _dynamoDbContext.LoadAsync<ContractDb>(query.Id).ConfigureAwait(false);
            return result?.ToDomain();
        }

        [LogCall]

        public async Task<Contract> PostNewContractAsync(ContractDb contract)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for target id {contract.TargetId}");
           _dynamoDbContext.SaveAsync(contract).GetAwaiter().GetResult();

            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for target id {contract.TargetId}");
            var result = await _dynamoDbContext.LoadAsync<ContractDb>(contract.Id).ConfigureAwait(false);

            return result.ToDomain();
        }

        [LogCall]
        public async Task<UpdateEntityResult<ContractDb>> PatchContract(Guid id, EditContractRequest contractRequestBody,
            string requestBody, int? ifMatch)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for id {id}");
            var existingContract = await _dynamoDbContext.LoadAsync<ContractDb>(id).ConfigureAwait(false);
            if (existingContract == null) return null;

            if (ifMatch != existingContract.VersionNumber)
                throw new VersionNumberConflictException(ifMatch, existingContract.VersionNumber);

            var response = _updater.UpdateEntity(existingContract, requestBody, contractRequestBody);

            if (response.NewValues.Any())
            {
                _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync to update id {id}");
                await _dynamoDbContext.SaveAsync(response.UpdatedEntity).ConfigureAwait(false);
            }

            return response;

        }
    }
}
