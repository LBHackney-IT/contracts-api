using System;

namespace ContractsApi.V1.Infrastructure.Exceptions
{
    public class DuplicateChargeException : Exception
    {
        //public Guid IncomingId { get; private set; }

        public DuplicateChargeException(Guid id)
            : base(string.Format("Charge with id ({0}) exists more than once.",
                 id.ToString()))
        {
            //IncomingId = id;
        }
    }
}
