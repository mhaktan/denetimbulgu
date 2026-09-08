using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.AuditPlans
{
    public class AuditPlanEntityTests
    {
        [Fact]
        public void AuditPlan_ShouldBeCreatable()
        {
            // Act
            var entity = new AuditPlan();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void AuditPlan_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new AuditPlan();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void AuditPlan_Period_ShouldAcceptValue()
        {
            var entity = new AuditPlan { Period = "Test Value" };
            entity.Period.Should().Be("Test Value");
        }

        [Fact]
        public void AuditPlan_ScopeDescription_ShouldAcceptValue()
        {
            var entity = new AuditPlan { ScopeDescription = "Test Value" };
            entity.ScopeDescription.Should().Be("Test Value");
        }

    }
}
