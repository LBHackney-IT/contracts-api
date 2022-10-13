using System.Threading.Tasks;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Gateways;
using ContractsApi.V1.UseCase.Interfaces;
using Hackney.Core.DynamoDb;
using Hackney.Core.Logging;

namespace ContractsApi.V1.UseCase
{
    public class GetContractsByTargetIdUseCase : IGetContractsByTargetIdUseCase
    {
        private readonly IContractGateway _gateway;

        public GetContractsByTargetIdUseCase(IContractGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<PagedResult<ContractResponseObject>> Execute(GetContractsQueryRequest query)
        {
            var response = await _gateway.GetContractsByTargetId(query).ConfigureAwait(false);

            return new PagedResult<ContractResponseObject>(response.Results.ToResponse(), response.PaginationDetails);
        }
    }
}
