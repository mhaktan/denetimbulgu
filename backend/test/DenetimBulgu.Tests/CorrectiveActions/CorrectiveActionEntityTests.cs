using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.CorrectiveActions
{
    public class CorrectiveActionEntityTests
    {
        [Fact]
        public void CorrectiveAction_ShouldBeCreatable()
        {
            // Act
            var entity = new CorrectiveAction();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void CorrectiveAction_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new CorrectiveAction();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void CorrectiveAction_RootCauseAnalysis_ShouldAcceptValue()
        {
            var entity = new CorrectiveAction { RootCauseAnalysis = "Test Value" };
            entity.RootCauseAnalysis.Should().Be("Test Value");
        }

        [Fact]
        public void CorrectiveAction_ActionDescription_ShouldAcceptValue()
        {
            var entity = new CorrectiveAction { ActionDescription = "Test Value" };
            entity.ActionDescription.Should().Be("Test Value");
        }

    }
}
