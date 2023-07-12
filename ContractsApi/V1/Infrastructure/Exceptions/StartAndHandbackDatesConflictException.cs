using System;

namespace ContractsApi.V1.Infrastructure.Exceptions
{
    public class StartAndHandbackDatesConflictException : Exception
    {
        public DateTime? ContractStartDate { get; private set; }
        public DateTime? ContractHandbackDate { get; private set; }

        public StartAndHandbackDatesConflictException(DateTime? startDate, DateTime? handbackDate)
            : base(string.Format("Handback date ({1}) cannot be prior to Start date ({0}).",
                (startDate is null) ? "{null}" : startDate.ToString(),
                (handbackDate is null) ? "{null}" : handbackDate.ToString()))
        {
            ContractStartDate = startDate;
            ContractHandbackDate = handbackDate;
        }
    }
}
