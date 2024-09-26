using System.Text.Json.Serialization;

namespace ContractsApi.V1.Domain
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Frequency
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApprovalStatus
    {
        PendingApproval,
        Approved,
        PendingReapproval
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApprovalStatusReason
    {
        ContractExtended,
        SuspensionLifted
    }
}
