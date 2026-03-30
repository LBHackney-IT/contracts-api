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
                EndReason = contractDb.EndReason,
                HandbackDate = contractDb.HandbackDate,
                ApprovalDate = contractDb.ApprovalDate,
                PaymentStartDate = contractDb.PaymentStartDate,
                RelatedPeople = contractDb.RelatedPeople,
                Charges = contractDb.Charges,
                VersionNumber = contractDb.VersionNumber,
                CostCentre = contractDb.CostCentre,
                Brma = contractDb.Brma,
                IsActive = contractDb.IsActive,
                ApprovalStatus = contractDb.ApprovalStatus,
                ApprovalStatusReason = contractDb.ApprovalStatusReason,
                HoldPayment = contractDb.HoldPayment,
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
                ReasonForSuspensionDate = contractDb.ReasonForSuspensionDate,
                ContractManagement = contractDb.ContractManagement
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
                EndReason = contract.EndReason,
                ApprovalDate = contract.ApprovalDate,
                PaymentStartDate = contract.PaymentStartDate,
                RelatedPeople = contract.RelatedPeople.ToList(),
                Charges = contract.Charges.ToList(),
                VersionNumber = contract.VersionNumber,
                CostCentre = contract.CostCentre,
                Brma = contract.Brma,
                IsActive = contract.IsActive,
                ApprovalStatus = contract.ApprovalStatus,
                ApprovalStatusReason = contract.ApprovalStatusReason,
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
                ReasonForSuspensionDate = contract.ReasonForSuspensionDate,
                ContractManagement = contract.ContractManagement
            };
        }
    }
}
