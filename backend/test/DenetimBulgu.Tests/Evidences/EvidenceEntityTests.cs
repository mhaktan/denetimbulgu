using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.Evidences
{
    public class EvidenceEntityTests
    {
        [Fact]
        public void Evidence_ShouldBeCreatable()
        {
            // Act
            var entity = new Evidence();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Evidence_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Evidence();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Evidence_Description_ShouldAcceptValue()
        {
            var entity = new Evidence { Description = "Test Value" };
            entity.Description.Should().Be("Test Value");
        }

    }
}
