using System.Collections.Generic;
using ContractsApi.V1.Domain;

namespace ContractsApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
