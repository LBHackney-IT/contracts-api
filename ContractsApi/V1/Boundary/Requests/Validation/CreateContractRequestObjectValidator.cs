using ContractsApi.V1.Domain;
using FluentValidation;
using System;

namespace ContractsApi.V1.Boundary.Requests.Validation
{
    public class CreateContractRequestObjectValidator : AbstractValidator<CreateContractRequestObject>
    {
        public CreateContractRequestObjectValidator()
        {
            RuleFor(x => x.ContractManagement.ContractHierarchy).IsInEnum()
                .When(x => x.ContractManagement != null)
                .WithMessage("The hierarchy provided is not valid");
            RuleFor(x => x.ContractManagement.ParentContractId).Empty()
                .When(x => x.ContractManagement?.ContractHierarchy != ContractHierarchy.ChildUnit)
                .WithMessage("ParentContractId must be empty for Blocks and Standalone Units");
            RuleFor(x => x.ContractManagement.ParentContractId).NotEmpty()
                .When(x => x.ContractManagement?.ContractHierarchy == ContractHierarchy.ChildUnit)
                .WithMessage("ParentContractId must be provided for Child Units");
        }
    }
}
