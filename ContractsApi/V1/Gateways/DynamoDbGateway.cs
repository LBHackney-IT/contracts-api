using System;
using System.Collections.Generic;
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
using Amazon.DynamoDBv2.DocumentModel;
using ContractsApi.V1.Infrastructure.Exceptions;
using Hackney.Core.DynamoDb;
using Hackney.Core.JWT;

namespace ContractsApi.V1.Gateways
{
    public class DynamoDbGateway : IContractGateway
    {
        private const int MAX_RESULTS = 10;
        private const string GETCONTRACTSBYTARGETIDINDEX = "ContractsByTargetId";
        private const string TARGETID = "targetId";

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
        public async Task<PagedResult<Contract>> GetContractsByTargetId(GetContractsQueryRequest query)
        {
            var pageSize = query.PageSize.HasValue ? query.PageSize.Value : MAX_RESULTS;
            var dbContracts = new List<ContractDb>();
            var table = _dynamoDbContext.GetTargetTable<ContractDb>();

            var queryConfig = new QueryOperationConfig
            {
                IndexName = GETCONTRACTSBYTARGETIDINDEX,
                Limit = pageSize,
                PaginationToken = PaginationDetails.DecodeToken(query.PaginationToken),
                Filter = new QueryFilter(TARGETID, QueryOperator.Equal, query.TargetId),
                BackwardSearch = !query.SortAscending
            };

            queryConfig.Filter.AddCondition("targetType", QueryOperator.Equal, query.TargetType);

            var search = table.Query(queryConfig);
            var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);

            var paginationToken = search.PaginationToken;
            if (resultsSet.Any())
            {
                dbContracts.AddRange(_dynamoDbContext.FromDocuments<ContractDb>(resultsSet));

                if (!string.IsNullOrEmpty(PaginationDetails.EncodeToken(paginationToken)))
                {
                    queryConfig.PaginationToken = paginationToken;
                    queryConfig.Limit = 1;
                    search = table.Query(queryConfig);
                    resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);
                    if (!resultsSet.Any())
                        paginationToken = null;
                }
            }

            return new PagedResult<Contract>(dbContracts.Select(x => x.ToDomain()), new PaginationDetails(
                paginationToken));
        }

        private async Task<int?> GetLatestContractNumber(Guid targetId, string targetType)
        {
            var newestContracts = await GetContractsByTargetId(
                new GetContractsQueryRequest
                {
                    TargetId = targetId,
                    TargetType = targetType,
                    PageSize = 1
                }
            );
            var newestContract = newestContracts?.Results?.FirstOrDefault();
            return newestContract?.TargetContractNumber;
        }

        [LogCall]

        public async Task<Contract> PostNewContractAsync(ContractDb contract)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for target id {contract.TargetId}");

            if (string.IsNullOrWhiteSpace(contract.Uprn))
            {
                throw new ArgumentException($"{nameof(contract.Uprn)} cannot be empty");
            }

            var dupes = contract.Charges.GroupBy(x => x.Id)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (dupes.Count() > 0)
                throw new DuplicateChargeException(dupes.First());

            var typeFrequencyDupes = contract.Charges.GroupBy(x => (x.Type, x.SubType, x.Frequency))
              .Where(g => g.Count() > 1)
              .SelectMany(y => y)
              .ToList();

            if (typeFrequencyDupes.Count() > 0)
                throw new DuplicateChargeFrequencyAndType(typeFrequencyDupes.First().Type, typeFrequencyDupes.First().SubType, typeFrequencyDupes.First().Frequency.ToString());

            var latestContractNumber = await GetLatestContractNumber(contract.TargetId, contract.TargetType);
            var newContractNumber = latestContractNumber == null ? 1 : (int) (latestContractNumber + 1);
            contract.TargetContractNumber = newContractNumber;

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
            if (contractRequestBody.HandbackDate != null)
            {
                if (existingContract.StartDate > contractRequestBody.HandbackDate || existingContract.StartDate is null)
                    throw new StartAndHandbackDatesConflictException(existingContract.StartDate, contractRequestBody.HandbackDate);
            }
            var response = _updater.UpdateEntity(existingContract, requestBody, contractRequestBody);

            if (response.NewValues.Any())
            {
                _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync to update id {id}");

                var dupes = response.UpdatedEntity.Charges.GroupBy(x => x.Id)
                  .Where(g => g.Count() > 1)
                  .Select(y => y.Key)
                  .ToList();

                if (dupes.Count() > 0)
                    throw new DuplicateChargeException(dupes.First());

                var typeFrequencyDupes = response.UpdatedEntity.Charges.GroupBy(x => (x.Type, x.SubType, x.Frequency))
                  .Where(g => g.Count() > 1)
                  .SelectMany(y => y)
                  .ToList();

                if (typeFrequencyDupes.Count() > 0)
                    throw new DuplicateChargeFrequencyAndType(typeFrequencyDupes.First().Type, typeFrequencyDupes.First().SubType, typeFrequencyDupes.First().Frequency.ToString());

                await _dynamoDbContext.SaveAsync(response.UpdatedEntity).ConfigureAwait(false);
            }

            return response;

        }
    }
}
