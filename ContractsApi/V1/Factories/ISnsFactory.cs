using ContractsApi.V1.Domain;
using Hackney.Core.JWT;
using Hackney.Core.Sns;

namespace ContractsApi.V1.Factories
{
    public interface ISnsFactory
    {
        EntityEventSns CreateContract(Contract contract, Token token);
    }
}
