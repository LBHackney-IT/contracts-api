using System;
using System.Threading.Tasks;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using Hackney.Core.JWT;

namespace ContractsApi.V1.UseCase.Interfaces
{
    public interface IPatchContractUseCase
    {
        Task<ContractResponseObject> ExecuteAsync(Guid id, EditContractRequest contract, string requestBody, Token token, int? ifMatch);
    }
}
