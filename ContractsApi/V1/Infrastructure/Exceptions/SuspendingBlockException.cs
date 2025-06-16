using System;

namespace ContractsApi.V1.Infrastructure.Exceptions
{
    public class SuspendingBlockException : Exception
    {
        public SuspendingBlockException()
            : base(string.Format("It is not possible to add a suspesion to blocks"))
        {
        }
    }
}
