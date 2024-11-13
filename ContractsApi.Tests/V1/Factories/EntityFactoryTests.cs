using ContractsApi.V1.Domain;
using ContractsApi.V1.Infrastructure;
using ContractsApi.V1.Factories;
using FluentAssertions;
using Xunit;
using AutoFixture;

namespace ContractsApi.Tests.V1.Factories
{
    public class EntityFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void ToDomainConvertsAllPropertiesCorrectly()
        {
            // Arrange
            var contractDb = _fixture.Create<ContractDb>();

            // Act
            var result = contractDb.ToDomain();

            // Assert
            result.Id.Should().Be(contractDb.Id);
            result.TargetId.Should().Be(contractDb.TargetId);
            result.TargetType.Should().Be(contractDb.TargetType);
            result.Uprn.Should().Be(contractDb.Uprn);
            result.TargetContractNumber.Should().Be(contractDb.TargetContractNumber);
            result.ContractType.Should().Be(contractDb.ContractType);
            result.StartDate.Should().Be(contractDb.StartDate);
            result.EndDate.Should().Be(contractDb.EndDate);
            result.EndReason.Should().Be(contractDb.EndReason);
            result.HandbackDate.Should().Be(contractDb.HandbackDate);
            result.ApprovalDate.Should().Be(contractDb.ApprovalDate);
            result.PaymentStartDate.Should().Be(contractDb.PaymentStartDate);
            result.RelatedPeople.Should().BeEquivalentTo(contractDb.RelatedPeople);
            result.Charges.Should().BeEquivalentTo(contractDb.Charges);
            result.VersionNumber.Should().Be(contractDb.VersionNumber);
            result.CostCentre.Should().Be(contractDb.CostCentre);
            result.Brma.Should().Be(contractDb.Brma);
            result.IsActive.Should().Be(contractDb.IsActive);
            result.ApprovalStatus.Should().Be(contractDb.ApprovalStatus);
            result.Stage.Should().Be(contractDb.Stage);
        }

        [Fact]
        public void ToDatabaseConvertsAllPropertiesCorrectly()
        {
            // Arrange
            var contract = _fixture.Create<Contract>();

            // Act
            var result = contract.ToDatabase();

            // Assert
            result.Id.Should().Be(contract.Id);
            result.TargetId.Should().Be(contract.TargetId);
            result.TargetType.Should().Be(contract.TargetType);
            result.Uprn.Should().Be(contract.Uprn);
            result.TargetContractNumber.Should().Be(contract.TargetContractNumber);
            result.ContractType.Should().Be(contract.ContractType);
            result.StartDate.Should().Be(contract.StartDate);
            result.EndDate.Should().Be(contract.EndDate);
            result.EndReason.Should().Be(contract.EndReason);
            result.HandbackDate.Should().BeNull();
            result.ApprovalDate.Should().Be(contract.ApprovalDate);
            result.PaymentStartDate.Should().Be(contract.PaymentStartDate);
            result.RelatedPeople.Should().BeEquivalentTo(contract.RelatedPeople);
            result.Charges.Should().BeEquivalentTo(contract.Charges);
            result.VersionNumber.Should().Be(contract.VersionNumber);
            result.CostCentre.Should().Be(contract.CostCentre);
            result.Brma.Should().Be(contract.Brma);
            result.IsActive.Should().Be(contract.IsActive);
            result.ApprovalStatus.Should().Be(contract.ApprovalStatus);
            result.Stage.Should().Be(contract.Stage);
        }
    }
}
