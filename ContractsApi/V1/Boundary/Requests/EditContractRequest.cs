using System;
using System.Collections.Generic;
using ContractsApi.V1.Domain;

namespace ContractsApi.V1.Boundary.Requests
{
    public class EditContractRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public IEnumerable<RelatedPeople> RelatedPeople { get; set; }
        public IEnumerable<Charges> Charges { get; set; }
    }
}
