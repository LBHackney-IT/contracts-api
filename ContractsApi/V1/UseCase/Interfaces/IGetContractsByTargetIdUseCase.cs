using System.Threading.Tasks;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using Hackney.Core.DynamoDb;

namespace ContractsApi.V1.UseCase.Interfaces
{
    public interface IGetContractsByTargetIdUseCase
    {
        Task<PagedResult<ContractResponseObject>> Execute(GetContractsQueryRequest query);
    }
}
