using ContractsApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace ContractsApi.V1.Boundary.Response
{
    public class ContractResponseObject
    {
        public Guid Id { get; set; }
        public string TargetType { get; set; }
        public string ContractType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public List<RelatedPeople> RelatedPeople { get; set; }
        public List<Charges> Charges { get; set; }
        public int? VersionNumber { get; set; }
    }
}