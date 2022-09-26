using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Gateways;
using ContractsApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using System.Threading.Tasks;

namespace ContractsApi.V1.UseCase
{
    public class GetContractByIdUseCase : IGetContractByIdUseCase
    {
        private readonly IContractGateway _gateway;

        public GetContractByIdUseCase(IContractGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<ContractResponseObject> Execute(ContractQueryRequest query)
        {
            var contract = await _gateway.GetEntityById(query).ConfigureAwait(false);

            return contract;
        }
    }
}
