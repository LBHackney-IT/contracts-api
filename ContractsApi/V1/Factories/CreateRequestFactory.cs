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
                Id = Guid.NewGuid(),
                TargetId = request.TargetId,
                TargetType = request.TargetType,
                Uprn = request.Uprn,
                ContractType = request.ContractType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                HandbackDate = request.HandbackDate,
                RelatedPeople = request.RelatedPeople.ToList(),
                Charges = request.Charges.ToList(),
                CostCentre = request.CostCentre,
                Brma = request.Brma,
                IsActive = request.IsActive,
                ApprovalStatus = request.ApprovalStatus,
                HoldPayment = request.HoldPayment,
                Stage = request.Stage,
                ApprovalDate = request.ApprovalDate,
                PaymentStartDate = request.PaymentStartDate,
                VersionNumber = request.VersionNumber,
                VatRegistrationNumber = request.VatRegistrationNumber,
                ReviewDate = request.ReviewDate,
                ExtensionDate = request.ExtensionDate,
                ReasonForExtensionDate = request.ReasonForExtensionDate,
                SelfBillingAgreement = request.SelfBillingAgreement,
                SelfBillingAgreementLinkToGoogleDrive = request.SelfBillingAgreementLinkToGoogleDrive,
                OptionToTax = request.OptionToTax,
                OptionToTaxLinkToGoogleDrive = request.OptionToTaxLinkToGoogleDrive,
                Rates = request.Rates,
                DefaultTenureType = request.DefaultTenureType,
                SuspensionDate = request.SuspensionDate,
                ReasonForSuspensionDate = request.ReasonForSuspensionDate,
                DraftContract = request.DraftContract
            };
        }
    }
}
