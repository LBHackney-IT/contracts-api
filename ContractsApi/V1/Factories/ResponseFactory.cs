using System.Collections.Generic;
using System.Linq;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;

namespace ContractsApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static ContractResponseObject ToResponse(this Contract contract)
        {
            return new ContractResponseObject
            {
                Id = contract.Id,
                TargetType = contract.TargetType,
                ContractType = contract.ContractType,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                RenewalDate = contract.RenewalDate,
                RelatedPeople = contract.RelatedPeople.ToList(),
                Charges = contract.Charges.ToList(),
                VersionNumber = contract.VersionNumber
            };
        }
    }
}
