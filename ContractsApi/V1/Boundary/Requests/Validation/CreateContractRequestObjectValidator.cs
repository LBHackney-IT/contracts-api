using FluentValidation;
using Hackney.Core.Validation;

namespace ContractsApi.V1.Boundary.Requests.Validation
{
    public class CreateContractRequestObjectValidator : AbstractValidator<CreateContractRequestObject>
    {
        public CreateContractRequestObjectValidator()
        {
            RuleFor(x => x.ContractManagement).NotEmpty()
                .WithMessage("The hierarchy of the contract must be present upon contract creation");
        }
    }
}
