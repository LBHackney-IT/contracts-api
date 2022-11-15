using System;
using System.Collections.Generic;

namespace ContractsApi.V1.Domain
{
    public class Contract
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public string TargetType { get; set; }
        public string ContractType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public IEnumerable<RelatedPeople> RelatedPeople { get; set; }
        public IEnumerable<Charges> Charges { get; set; }
        public int? VersionNumber { get; set; }
        public string FundingSource { get; set; }
        public string CostCentre { get; set; }
        public string LhaArea { get; set; }
        public decimal? LhaRate { get; set; }
    }
}
