using System;

namespace ContractsApi.V1.Infrastructure.Exceptions
{
    public class DuplicateChargeFrequencyAndType : Exception
    {
        public DuplicateChargeFrequencyAndType(string incomingType, string incomingSubType, string incomingFrequency)
            : base(string.Format("Type ({0}) and sub type ({1}) with frequency ({2}) exists more than once.",
                (incomingType is null) ? "{null}" : incomingType.ToString(),
                (incomingSubType is null) ? "{null}" : incomingSubType.ToString(),
                (incomingFrequency is null) ? "{null}" : incomingFrequency.ToString()))
        {

        }
    }
}
