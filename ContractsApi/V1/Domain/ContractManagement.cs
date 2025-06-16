using System;

namespace ContractsApi.V1.Domain
{
    public class ContractManagement
    {
        public AssetHierarchy AssetHierarchy { get; set; }
        public Guid? ParentAssetId { get; set; }
    }
}
