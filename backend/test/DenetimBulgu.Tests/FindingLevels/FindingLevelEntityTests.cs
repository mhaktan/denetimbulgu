using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.FindingLevels
{
    public class FindingLevelEntityTests
    {
        [Fact]
        public void FindingLevel_ShouldBeCreatable()
        {
            // Act
            var entity = new FindingLevel();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void FindingLevel_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new FindingLevel();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void FindingLevel_Code_ShouldAcceptValue()
        {
            var entity = new FindingLevel { Code = "Test Value" };
            entity.Code.Should().Be("Test Value");
        }

        [Fact]
        public void FindingLevel_Name_ShouldAcceptValue()
        {
            var entity = new FindingLevel { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
