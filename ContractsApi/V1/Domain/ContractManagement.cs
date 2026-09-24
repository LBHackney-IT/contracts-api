using System;

namespace ContractsApi.V1.Domain
{
    public class ContractManagement
    {
        public ContractHierarchy ContractHierarchy { get; set; }
        public Guid? ParentContractId { get; set; }
    }
}
