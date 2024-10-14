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
                ContractNumber = contract.ContractNumber,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                EndReason = contract.EndReason,
                HandbackDate = contract.HandbackDate,
                RelatedPeople = contract.RelatedPeople.ToList(),
                Charges = contract.Charges.ToList(),
                CostCentre = contract.CostCentre,
                Brma = contract.Brma,
                IsActive = contract.IsActive,
                IsApproved = contract.IsApproved,
                ApprovalStatus = contract.ApprovalStatus,
                ApprovalStatusReason = contract.ApprovalStatusReason,
                HoldPayment = contract.HoldPayment,
                Stage = contract.Stage,
                ApprovalDate = contract.ApprovalDate,
                PaymentStartDate = contract.PaymentStartDate,
                VersionNumber = contract.VersionNumber,
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

        public static List<ContractResponseObject> ToResponse(this IEnumerable<Contract> domainList)
        {
            if (domainList is null) return new List<ContractResponseObject>();

            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
