using System.Collections.Generic;
using System.Threading.Tasks;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;

namespace ContractsApi.V1.Gateways
{
    public interface IContractGateway
    {
        Task<ContractResponseObject> GetEntityById(ContractQueryRequest query);

    }
}
