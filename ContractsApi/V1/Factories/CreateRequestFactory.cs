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
                ContractType = request.ContractType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RenewalDate = request.RenewalDate,
                RelatedPeople = request.RelatedPeople.ToList(),
                Charges = request.Charges.ToList(),
                FundingSource = request.FundingSource,
                CostCentre = request.CostCentre,
                LhaArea = request.LhaArea,
                LhaRate = request.LhaRate,
                IsActive = request.IsActive,
                IsVATRegistered = request.IsVATRegistered,
                Stage = request.Stage,
                ApprovalDate = request.ApprovalDate,
                VersionNumber = request.VersionNumber,
                VATRegistrationNumber = request.VATRegistrationNumber,
                ReviewDate = request.ReviewDate,
                ExtensionDate = request.ExtensionDate,
                ReasonForExtensionDate = request.ReasonForExtensionDate,
                SelfBillingAgreement = request.SelfBillingAgreement,
                SelfBillingAgreementLinkToGoogleDrive = request.SelfBillingAgreementLinkToGoogleDrive,
                OptionToTax = request.OptionToTax,
                OptionToTaxLinkToGoogleDrive = request.OptionToTaxLinkToGoogleDrive,
                Rates = request.Rates
            };
        }
    }
}
