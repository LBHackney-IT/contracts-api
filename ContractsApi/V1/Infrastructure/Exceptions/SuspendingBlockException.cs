using System;

namespace ContractsApi.V1.Infrastructure.Exceptions
{
    public class SuspendingBlockException : Exception
    {
        public SuspendingBlockException()
            : base("It is not possible to add a suspension to blocks")
        {
        }
    }
}
