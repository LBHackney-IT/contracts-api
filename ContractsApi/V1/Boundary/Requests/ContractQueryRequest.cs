using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json.Serialization;

namespace ContractsApi.V1.Boundary.Requests
{
    public class ContractQueryRequest
    {
        [FromRoute(Name = "id")]
        [JsonRequired] public Guid Id { get; set; }
    }
}
