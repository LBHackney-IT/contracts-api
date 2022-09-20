using ContractsApi.V1.Boundary.Response;

namespace ContractsApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        ResponseObject Execute(int id);
    }
}
