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
        [Fact]
        public void RequestShouldErrorIfHierarchyIsBlockOrStandaloneAndParentContractIdIsProvided()
        {
            var model = new CreateContractRequestObject()
            {
                ContractManagement = new()
                {
                    ContractHierarchy = (ContractHierarchy) 1,
                    ParentContractId = new Guid()
                }
            };
            var result = _ccrov.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ContractManagement.ParentContractId)
            .WithErrorMessage("ParentContractId must be empty for Blocks and Standalone Units");
        }
        [Fact]
        public void RequestShouldErrorIfHierarchyIsChildUnitAndParentContractIdIsNotProvided()
        {
            var model = new CreateContractRequestObject()
            {
                ContractManagement = new()
                {
                    ContractHierarchy = (ContractHierarchy) 2,
                    ParentContractId = null
                }
            };
            var result = _ccrov.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ContractManagement.ParentContractId)
            .WithErrorMessage("ParentContractId must be provided for Child Units");
        }
    }
}
