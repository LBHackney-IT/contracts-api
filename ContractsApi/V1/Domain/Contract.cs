using System;
using System.Collections.Generic;

namespace ContractsApi.V1.Domain
{
    public class Contract
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public string TargetType { get; set; }
        public string Uprn { get; set; }
        public int TargetContractNumber { get; set; }
        public string ContractType { get; set; }
        public string ContractNumber => $"{Uprn}N{TargetContractNumber}";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EndReason { get; set; }
        public DateTime? HandbackDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? PaymentStartDate { get; set; }
        public IEnumerable<RelatedPeople> RelatedPeople { get; set; }
        public IEnumerable<Charges> Charges { get; set; }
        public int? VersionNumber { get; set; }
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
        public ApprovalStatus ApprovalStatus { get; set; }
        public Frequency Rates { get; set; }
        public TenureType DefaultTenureType { get; set; }
        public DateTime? SuspensionDate { get; set; }
        public string ReasonForSuspensionDate { get; set; }
    }
}
