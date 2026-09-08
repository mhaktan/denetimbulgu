using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.Findings
{
    public class FindingEntityTests
    {
        [Fact]
        public void Finding_ShouldBeCreatable()
        {
            // Act
            var entity = new Finding();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Finding_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Finding();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Finding_Title_ShouldAcceptValue()
        {
            var entity = new Finding { Title = "Test Value" };
            entity.Title.Should().Be("Test Value");
        }

        [Fact]
        public void Finding_Description_ShouldAcceptValue()
        {
            var entity = new Finding { Description = "Test Value" };
            entity.Description.Should().Be("Test Value");
        }

    }
}
