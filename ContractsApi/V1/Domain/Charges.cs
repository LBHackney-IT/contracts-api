using System;

namespace ContractsApi.V1.Domain
{
    public class Charges
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public Frequency Frequency { get; set; }
        public decimal? Amount { get; set; }
        public string VatRate { get; set; }
    }
}
