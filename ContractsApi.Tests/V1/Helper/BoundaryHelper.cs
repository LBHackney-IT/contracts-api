using System;
using AutoFixture;
using ContractsApi.V1.Boundary.Requests;

namespace ContractsApi.Tests.V1.Helper
{
    public static class BoundaryHelper
    {
        private static readonly Fixture _fixture = new Fixture();
        public static ContractQueryRequest ConstructRequest(Guid? id = null)
        {
            return new ContractQueryRequest() { Id = id ?? Guid.NewGuid() };
        }

        public static CreateContractRequestObject ConstructPostRequest()
        {
            return _fixture.Build<CreateContractRequestObject>()
                .Create();
        }
    }
}
