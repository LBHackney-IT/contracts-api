using ContractsApi.V1.Domain;
using ContractsApi.V1.Infrastructure;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;

namespace ContractsApi.V1.Factories
{
    public class ContractSnsFactory : ISnsFactory
    {
        public EntityEventSns CreateContract(Contract contract, Token token)
        {
            return new EntityEventSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = contract.Id,
                Id = Guid.NewGuid(),
                EventType = CreateContractEventConstants.EVENTTYPE,
                Version = CreateContractEventConstants.V1_VERSION,
                SourceDomain = CreateContractEventConstants.SOURCE_DOMAIN,
                SourceSystem = CreateContractEventConstants.SOURCE_SYSTEM,
                EventData = new EventData
                {
                    NewData = contract
                },
                User = new User { Name = token.Name, Email = token.Email }
            };
        }

        public EntityEventSns UpdateContract(UpdateEntityResult<ContractDb> updateResult, Token token)
        {
            return new EntityEventSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = updateResult.UpdatedEntity.Id,
                Id = Guid.NewGuid(),
                EventType = UpdateContractConstants.EVENTTYPE,
                Version = UpdateContractConstants.V1_VERSION,
                SourceDomain = UpdateContractConstants.SOURCE_DOMAIN,
                SourceSystem = UpdateContractConstants.SOURCE_SYSTEM,
                EventData = new EventData
                {
                    NewData = updateResult.NewValues,
                    OldData = updateResult.OldValues
                },
                User = new User { Name = token.Name, Email = token.Email }
            };
        }
    }
}
