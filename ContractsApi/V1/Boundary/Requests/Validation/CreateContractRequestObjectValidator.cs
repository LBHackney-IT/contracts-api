using FluentValidation;
using System;

namespace ContractsApi.V1.Boundary.Requests.Validation
{
    public class CreateContractRequestObjectValidator : AbstractValidator<CreateContractRequestObject>
    {
        public CreateContractRequestObjectValidator()
        {
            RuleFor(x => x.ContractManagement).NotEmpty()
                .WithMessage("ContractManagement must be present upon contract creation");
            RuleFor(x => x.ContractManagement.ContractHierarchy).IsInEnum()
                .When(x => x.ContractManagement != null)
                .WithMessage("The hierarchy provided is not valid");
        }
    }
}
