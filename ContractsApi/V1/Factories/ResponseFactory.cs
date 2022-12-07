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
                TargetId = contract.TargetId,
                TargetType = contract.TargetType,
                ContractType = contract.ContractType,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                RenewalDate = contract.RenewalDate,
                RelatedPeople = contract.RelatedPeople.ToList(),
                Charges = contract.Charges.ToList(),
                FundingSource = contract.FundingSource,
                CostCentre = contract.CostCentre,
                LhaArea = contract.LhaArea,
                LhaRate = contract.LhaRate,
                IsActive = contract.IsActive,
                IsVATRegistered = contract.IsVATRegistered,
                Stage = contract.Stage,
                VersionNumber = contract.VersionNumber
            };
        }

        public static List<ContractResponseObject> ToResponse(this IEnumerable<Contract> domainList)
        {
            if (domainList is null) return new List<ContractResponseObject>();

            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
