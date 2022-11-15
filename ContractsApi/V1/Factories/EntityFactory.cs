using System.Linq;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Infrastructure;

namespace ContractsApi.V1.Factories
{
    public static class EntityFactory
    {

        public static Contract ToDomain(this ContractDb contractDb)
        {
            return new Contract
            {
                Id = contractDb.Id,
                TargetId = contractDb.TargetId,
                TargetType = contractDb.TargetType,
                ContractType = contractDb.ContractType,
                StartDate = contractDb.StartDate,
                EndDate = contractDb.EndDate,
                RenewalDate = contractDb.RenewalDate,
                RelatedPeople = contractDb.RelatedPeople,
                Charges = contractDb.Charges,
                VersionNumber = contractDb.VersionNumber,
                FundingSource = contractDb.FundingSource,
                CostCentre = contractDb.CostCentre,
                LhaArea = contractDb.LhaArea,
                LhaRate = contractDb.LhaRate,
            };
        }

        public static ContractDb ToDatabase(this Contract contract)
        {

            return new ContractDb()
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
                VersionNumber = contract.VersionNumber,
                FundingSource = contract.FundingSource,
                CostCentre = contract.CostCentre,
                LhaArea = contract.LhaArea,
                LhaRate = contract.LhaRate,
            };
        }
    }
}
