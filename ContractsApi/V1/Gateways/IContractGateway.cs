using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Infrastructure;
using Hackney.Core.JWT;

namespace ContractsApi.V1.Gateways
{
    public interface IContractGateway
    {
        Task<Contract> GetContractById(ContractQueryRequest query);
        Task<Contract> PostNewContractAsync(ContractDb contractDb);

        Task<UpdateEntityResult<ContractDb>> PatchContract(Guid id, EditContractRequest contractRequestBody, string requestBody, int? ifMatch);
    }
}
