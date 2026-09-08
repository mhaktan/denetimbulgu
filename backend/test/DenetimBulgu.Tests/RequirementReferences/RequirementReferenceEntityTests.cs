using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.RequirementReferences
{
    public class RequirementReferenceEntityTests
    {
        [Fact]
        public void RequirementReference_ShouldBeCreatable()
        {
            // Act
            var entity = new RequirementReference();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void RequirementReference_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new RequirementReference();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void RequirementReference_Code_ShouldAcceptValue()
        {
            var entity = new RequirementReference { Code = "Test Value" };
            entity.Code.Should().Be("Test Value");
        }

        [Fact]
        public void RequirementReference_Title_ShouldAcceptValue()
        {
            var entity = new RequirementReference { Title = "Test Value" };
            entity.Title.Should().Be("Test Value");
        }

    }
}
