using System;
using System.Text.Json.Serialization;

namespace ContractsApi.V1.Boundary.Requests
{
    public class GetContractsQueryRequest
    {
        public Guid TargetId { get; set; }
        [JsonRequired] public string TargetType { get; set; }

        /// <summary>
        /// If true, return oldest contracts first
        /// otherwise return newest contracts first
        /// </summary>
        public bool SortAscending { get; set; }
        public string PaginationToken { get; set; }
        public int? PageSize { get; set; }
    }
}
