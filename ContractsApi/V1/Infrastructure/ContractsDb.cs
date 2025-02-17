using Amazon.DynamoDBv2.DataModel;
using Hackney.Core.DynamoDb.Converters;
using System;
using System.Collections.Generic;
using ContractsApi.V1.Domain;

namespace ContractsApi.V1.Infrastructure
{
    [DynamoDBTable("Contracts", LowerCamelCaseProperties = true)]
    public class ContractDb
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public Guid TargetId { get; set; }

        [DynamoDBProperty]
        public string TargetType { get; set; }

        [DynamoDBProperty]
        public string Uprn { get; set; }

        [DynamoDBProperty]
        public int TargetContractNumber { get; set; }

        [DynamoDBProperty]
        public string ContractType { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? StartDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? EndDate { get; set; }

        [DynamoDBProperty]
        public string EndReason { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? HandbackDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? ApprovalDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? PaymentStartDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<RelatedPeople>))]
        public List<RelatedPeople> RelatedPeople { get; set; } = new List<RelatedPeople>();

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<Charges>))]
        public List<Charges> Charges { get; set; } = new List<Charges>();

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }

        [DynamoDBProperty]
        public string CostCentre { get; set; }

        [DynamoDBProperty]
        public string Brma { get; set; }

        [DynamoDBProperty]
        public bool? IsActive { get; set; }

        [DynamoDBProperty]
        public string ApprovalStatusReason { get; set; }

        [DynamoDBProperty]
        public bool? HoldPayment { get; set; }

        [DynamoDBProperty]
        public int? Stage { get; set; }

        [DynamoDBProperty]
        public string VatRegistrationNumber { get; set; }

        [DynamoDBProperty]
        public DateTime? ReviewDate { get; set; }

        [DynamoDBProperty]
        public DateTime? ExtensionDate { get; set; }

        [DynamoDBProperty]
        public string ReasonForExtensionDate { get; set; }

        [DynamoDBProperty]
        public bool? SelfBillingAgreement { get; set; }

        [DynamoDBProperty]
        public string SelfBillingAgreementLinkToGoogleDrive { get; set; }

        [DynamoDBProperty]
        public bool? OptionToTax { get; set; }

        [DynamoDBProperty]
        public string OptionToTaxLinkToGoogleDrive { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbEnumConverter<ApprovalStatus>))]
        public ApprovalStatus ApprovalStatus { get; set; }

        [DynamoDBProperty]
        public Frequency Rates { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<TenureType>))]
        public TenureType DefaultTenureType { get; set; }

        [DynamoDBProperty]
        public DateTime? SuspensionDate { get; set; }

        [DynamoDBProperty]
        public string ReasonForSuspensionDate { get; set; }
    }
}
