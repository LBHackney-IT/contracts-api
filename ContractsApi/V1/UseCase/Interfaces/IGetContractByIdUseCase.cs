using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;
using System.Threading.Tasks;

namespace ContractsApi.V1.UseCase.Interfaces
{
    public interface IGetContractByIdUseCase
    {
        Task<ContractResponseObject> Execute(ContractQueryRequest query);
    }
}
