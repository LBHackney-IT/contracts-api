using Amazon.DynamoDBv2.DataModel;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Infrastructure;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContractsApi.V1.Gateways
{
    public class DynamoDbGateway : IContractGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<DynamoDbGateway> _logger;


        public DynamoDbGateway(IDynamoDBContext dynamoDbContext, ILogger<DynamoDbGateway> logger)
        {
            _dynamoDbContext = dynamoDbContext;
            _logger = logger;
        }

        [LogCall]
        public async Task<Contract> GetEntityById(ContractQueryRequest query)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id}");

            var result = await _dynamoDbContext.LoadAsync<ContractDb>(query.Id).ConfigureAwait(false);
            return result?.ToDomain();
        }

        [LogCall]

        public async Task<Contract> PostNewContractAsync(ContractDb contract)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for id {contract.Id}");
            _dynamoDbContext.SaveAsync(contract).GetAwaiter().GetResult();

            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {contract.Id}");
            var result = await _dynamoDbContext.LoadAsync<ContractDb>(contract.Id).ConfigureAwait(false);

            return result?.ToDomain();
        }
    }
}
