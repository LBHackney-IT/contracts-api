using ContractsApi.V1.Domain;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ContractsApi.V1.Boundary.Requests
{
    public class EditContractRequest
    {
        public string ContractType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EndReason { get; set; }
        public DateTime? HandbackDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? PaymentStartDate { get; set; }
        public IEnumerable<RelatedPeople> RelatedPeople { get; set; }
        public IEnumerable<Charges> Charges { get; set; }
        public string CostCentre { get; set; }
        public string Brma { get; set; }
        public bool? IsActive { get; set; }
        public string ApprovalStatusReason { get; set; }
        public bool? HoldPayment { get; set; }
        public int? Stage { get; set; }
        public string VatRegistrationNumber { get; set; }
        public DateTime? ReviewDate { get; set; }
        public DateTime? ExtensionDate { get; set; }
        public string ReasonForExtensionDate { get; set; }
        public bool? SelfBillingAgreement { get; set; }
        public string SelfBillingAgreementLinkToGoogleDrive { get; set; }
        public bool? OptionToTax { get; set; }
        public string OptionToTaxLinkToGoogleDrive { get; set; }
        [JsonRequired] public ApprovalStatus ApprovalStatus { get; set; }
        [JsonRequired] public Frequency Rates { get; set; }
        public TenureType DefaultTenureType { get; set; }
        public DateTime? SuspensionDate { get; set; }
        public string ReasonForSuspensionDate { get; set; }
    }

    public class CustomEditContractValidation : AbstractValidator<EditContractRequest>
    {
        public CustomEditContractValidation()
        {
            RuleFor(x => x.HandbackDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate != null && x.HandbackDate != null)
                .WithMessage("Handback date cannot be prior to Start date");
        }
    }
}
