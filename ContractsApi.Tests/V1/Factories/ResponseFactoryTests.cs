using ContractsApi.V1.Domain;
using ContractsApi.V1.Factories;
using FluentAssertions;
using AutoFixture;
using Xunit;
using Microsoft.VisualBasic;


namespace ContractsApi.Tests.V1.Factories
{
    public class ResponseFactoryTests
    {
        private Fixture _fixture = new Fixture();

        [Fact]
        public void CanMapContractDomainToResponse()
        {
            // Arrange
            var contract = _fixture.Create<Contract>();

            // Act
            var response = contract.ToResponse();

            // Assert
            response.Id.Should().Be(contract.Id);
            response.TargetId.Should().Be(contract.TargetId);
            response.TargetType.Should().Be(contract.TargetType);
            response.ContractType.Should().Be(contract.ContractType);
            response.ContractNumber.Should().Be(contract.ContractNumber);
            response.StartDate.Should().Be(contract.StartDate);
            response.EndDate.Should().Be(contract.EndDate);
            response.EndReason.Should().Be(contract.EndReason);
            response.HandbackDate.Should().Be(contract.HandbackDate);
            response.RelatedPeople.Should().HaveCountGreaterThan(1);
            response.Charges.Should().HaveCount(3);
            response.CostCentre.Should().Be(contract.CostCentre);
            response.Brma.Should().Be(contract.Brma);
            response.IsActive.Should().Be(contract.IsActive);
            response.ApprovalStatus.Should().Be(contract.ApprovalStatus);
            response.ApprovalStatusReason.Should().Be(contract.ApprovalStatusReason);
            response.HoldPayment.Should().Be(contract.HoldPayment);
            response.Stage.Should().Be(contract.Stage);
            response.ApprovalDate.Should().Be(contract.ApprovalDate);
            response.PaymentStartDate.Should().Be(contract.PaymentStartDate);
            response.VersionNumber.Should().Be(contract.VersionNumber);
            response.VatRegistrationNumber.Should().Be(contract.VatRegistrationNumber);
            response.ReviewDate.Should().Be(contract.ReviewDate);
            response.ExtensionDate.Should().Be(contract.ExtensionDate);
            response.ReasonForExtensionDate.Should().Be(contract.ReasonForExtensionDate);
            response.SelfBillingAgreement.Should().Be(contract.SelfBillingAgreement);
            response.SelfBillingAgreementLinkToGoogleDrive.Should().Be(contract.SelfBillingAgreementLinkToGoogleDrive);
            response.OptionToTax.Should().Be(contract.OptionToTax);
            response.OptionToTaxLinkToGoogleDrive.Should().Be(contract.OptionToTaxLinkToGoogleDrive);
            response.Rates.Should().Be(contract.Rates);
            response.DefaultTenureType.Should().Be(contract.DefaultTenureType);
            response.SuspensionDate.Should().Be(contract.SuspensionDate);
            response.ReasonForExtensionDate.Should().Be(contract.ReasonForExtensionDate);
        }
    }
}
