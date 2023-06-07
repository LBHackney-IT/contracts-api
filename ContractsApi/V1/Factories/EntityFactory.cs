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
                Uprn = contractDb.Uprn,
                TargetContractNumber = contractDb.TargetContractNumber,
                ContractType = contractDb.ContractType,
                StartDate = contractDb.StartDate,
                EndDate = contractDb.EndDate,
                RenewalDate = contractDb.RenewalDate,
                ApprovalDate = contractDb.ApprovalDate,
                RelatedPeople = contractDb.RelatedPeople,
                Charges = contractDb.Charges,
                VersionNumber = contractDb.VersionNumber,
                CostCentre = contractDb.CostCentre,
                LhaArea = contractDb.LhaArea,
                LhaRate = contractDb.LhaRate,
                IsActive = contractDb.IsActive,
                IsVATRegistered = contractDb.IsVATRegistered,
                Stage = contractDb.Stage,
                VatRegistrationNumber = contractDb.VatRegistrationNumber,
                ReviewDate = contractDb.ReviewDate,
                ExtensionDate = contractDb.ExtensionDate,
                ReasonForExtensionDate = contractDb.ReasonForExtensionDate,
                SelfBillingAgreement = contractDb.SelfBillingAgreement,
                SelfBillingAgreementLinkToGoogleDrive = contractDb.SelfBillingAgreementLinkToGoogleDrive,
                OptionToTax = contractDb.OptionToTax,
                OptionToTaxLinkToGoogleDrive = contractDb.OptionToTaxLinkToGoogleDrive,
                Rates = contractDb.Rates,
                DefaultTenureType = contractDb.DefaultTenureType,
                SuspensionDate = contractDb.SuspensionDate,
                ReasonForSuspensionDate = contractDb.ReasonForSuspensionDate
            };
        }

        public static ContractDb ToDatabase(this Contract contract)
        {

            return new ContractDb()
            {
                Id = contract.Id,
                TargetId = contract.TargetId,
                TargetType = contract.TargetType,
                Uprn = contract.Uprn,
                TargetContractNumber = contract.TargetContractNumber,
                ContractType = contract.ContractType,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                RenewalDate = contract.RenewalDate,
                ApprovalDate = contract.ApprovalDate,
                RelatedPeople = contract.RelatedPeople.ToList(),
                Charges = contract.Charges.ToList(),
                VersionNumber = contract.VersionNumber,
                CostCentre = contract.CostCentre,
                LhaArea = contract.LhaArea,
                LhaRate = contract.LhaRate,
                IsActive = contract.IsActive,
                IsVATRegistered = contract.IsVATRegistered,
                Stage = contract.Stage,
                VatRegistrationNumber = contract.VatRegistrationNumber,
                ReviewDate = contract.ReviewDate,
                ExtensionDate = contract.ExtensionDate,
                ReasonForExtensionDate = contract.ReasonForExtensionDate,
                SelfBillingAgreement = contract.SelfBillingAgreement,
                SelfBillingAgreementLinkToGoogleDrive = contract.SelfBillingAgreementLinkToGoogleDrive,
                OptionToTax = contract.OptionToTax,
                OptionToTaxLinkToGoogleDrive = contract.OptionToTaxLinkToGoogleDrive,
                Rates = contract.Rates,
                DefaultTenureType = contract.DefaultTenureType,
                SuspensionDate = contract.SuspensionDate,
                ReasonForSuspensionDate = contract.ReasonForSuspensionDate
            };
        }
    }
}
