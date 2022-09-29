using System.Collections.Generic;
using System.Threading.Tasks;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Infrastructure;

namespace ContractsApi.V1.Gateways
{
    public interface IContractGateway
    {
        Task<Contract> GetEntityById(ContractQueryRequest query);
        Task<Contract> PostNewContractAsync(ContractDb contractDb);
    }
}
