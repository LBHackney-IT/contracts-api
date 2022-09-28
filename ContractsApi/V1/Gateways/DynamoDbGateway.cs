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

        public List<Entity> GetAll()
        {
            return new List<Entity>();
        }

        [LogCall]
        public async Task<Contract> GetEntityById(ContractQueryRequest query)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id}");

            var result = await _dynamoDbContext.LoadAsync<Contract>(query.Id).ConfigureAwait(false);
            return result;
        }

        [LogCall]

        public async Task<Contract> PostNewContractAsync(CreateContractRequestObject createContractRequestObject)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for id {createContractRequestObject.TargetId}");
            _dynamoDbContext.SaveAsync(createContractRequestObject).GetAwaiter().GetResult();

            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {createContractRequestObject.TargetId}");
            var result = await _dynamoDbContext.LoadAsync<Contract>(createContractRequestObject.TargetId).ConfigureAwait(false);

            return result;
        }
    }
}
