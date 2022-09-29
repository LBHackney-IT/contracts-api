using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;
using Hackney.Core.JWT;
using System.Threading.Tasks;

namespace ContractsApi.V1.UseCase.Interfaces
{
    public interface IPostNewContractUseCase
    {
        Task<ContractResponseObject> ExecuteAsync(CreateContractRequestObject createTenureRequestObject, Token token);
    }
}
