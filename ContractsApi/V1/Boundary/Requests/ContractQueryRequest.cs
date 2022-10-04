using Microsoft.AspNetCore.Mvc;
using System;

namespace ContractsApi.V1.Boundary.Requests
{
    public class ContractQueryRequest
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}
