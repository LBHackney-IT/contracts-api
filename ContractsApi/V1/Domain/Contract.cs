using System;
using System.Collections.Generic;

namespace ContractsApi.V1.Domain
{
    public class Contract
    {
        public Guid Id { get; set; }
        public string TargetType { get; set; }
        public string ContractType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public IEnumerable<RelatedPeople> RelatedPeople { get; set; }
        public IEnumerable<Charges> Charges { get; set; }
        public int? VersionNumber { get; set; }
    }
}
