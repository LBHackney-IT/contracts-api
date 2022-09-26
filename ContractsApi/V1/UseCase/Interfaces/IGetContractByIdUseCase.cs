using ContractsApi.V1.Boundary.Response;

namespace ContractsApi.V1.UseCase.Interfaces
{
    public interface IGetContractByIdUseCase
    {
        ResponseObject Execute(int id);
    }
}
