using System;

namespace ContractsApi.V1.Boundary.Requests
{
    public class GetContractsQueryRequest
    {
        public Guid TargetId { get; set; }
        public string TargetType { get; set; }
        public string PaginationToken { get; set; }
        public int? PageSize { get; set; }
    }
}
