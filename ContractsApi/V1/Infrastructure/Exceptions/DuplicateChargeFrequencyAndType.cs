using System;

namespace ContractsApi.V1.Infrastructure.Exceptions
{
    public class DuplicateChargeFrequencyAndType : Exception
    {
        public DuplicateChargeFrequencyAndType(string incomingType, string incomingFrequency)
            : base(string.Format("Type ({0}) and frequency ({1}) exists more than once.",
                (incomingType is null) ? "{null}" : incomingType.ToString(),
                (incomingFrequency is null) ? "{null}" : incomingFrequency.ToString()))
        {

        }
    }
}
