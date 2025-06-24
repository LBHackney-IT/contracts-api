using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Requests.Validation;
using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using System;
using System.Linq;
using Xunit;
using ContractsApi.V1.Domain;

namespace ContractsApi.Tests.V1.Boundary.Request.Validation
{
#nullable enable
    public class CreateContractRequestObjectValidatorTests
    {
        private readonly CreateContractRequestObjectValidator _ccrov;
        private readonly Fixture _fixture = new();

        public CreateContractRequestObjectValidatorTests()
        {
            _ccrov = new CreateContractRequestObjectValidator();
        }

        [Fact]
        public void RequestShouldErrorForNoContractHierarchy()
        {
            var model = new CreateContractRequestObject() { ContractManagement = null };
            var result = _ccrov.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ContractManagement)
            .WithErrorMessage("The hierarchy of the contract must be present upon contract creation");
        }
        [Fact]
        public void RequestShouldErrorForInvalidEnumValue()
        {
            var model = new CreateContractRequestObject()
            {
                ContractManagement = new()
                {
                    ContractHierarchy = (ContractHierarchy) 9
                }
            };
            var result = _ccrov.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ContractManagement.ContractHierarchy)
            .WithErrorMessage("The hierarchy provided is not valid");
        }
    }
}
