using System;
using System.Linq;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Infrastructure;

namespace ContractsApi.V1.Factories
{
    public static class CreateRequestFactory
    {
        public static ContractDb ToDatabase(this CreateContractRequestObject request)
        {

            return new ContractDb()
            {
                Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id,
                TargetType = request.TargetType,
                ContractType = request.ContractType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RenewalDate = request.RenewalDate,
                RelatedPeople = request.RelatedPeople.ToList(),
                Charges = request.Charges.ToList(),
                VersionNumber = request.VersionNumber
            };
        }
    }
}
