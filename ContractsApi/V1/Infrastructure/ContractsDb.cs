using Amazon.DynamoDBv2.DataModel;
using Hackney.Core.DynamoDb.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string ContractType { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? StartDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? EndDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? RenewalDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<RelatedPeople>))]
        public List<RelatedPeople> RelatedPeople { get; set; } = new List<RelatedPeople>();

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<Charges>))]
        public List<Charges> Charges { get; set; } = new List<Charges>();

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
        [DynamoDBProperty]
        public string FundingSource { get; set; }
        [DynamoDBProperty]
        public string CostCentre { get; set; }
        [DynamoDBProperty]
        public string LhaArea { get; set; }
        [DynamoDBProperty]
        public decimal? LhaRate { get; set; }
        [DynamoDBProperty]
        public bool? IsActive { get; set; }
        [DynamoDBProperty]
        public bool? IsVATRegistered { get; set; }
        [DynamoDBProperty]
        public int? Stage { get; set; }
    }
}
