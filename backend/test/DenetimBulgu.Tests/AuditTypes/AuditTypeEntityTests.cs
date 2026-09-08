using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.AuditTypes
{
    public class AuditTypeEntityTests
    {
        [Fact]
        public void AuditType_ShouldBeCreatable()
        {
            // Act
            var entity = new AuditType();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void AuditType_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new AuditType();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void AuditType_Code_ShouldAcceptValue()
        {
            var entity = new AuditType { Code = "Test Value" };
            entity.Code.Should().Be("Test Value");
        }

        [Fact]
        public void AuditType_Name_ShouldAcceptValue()
        {
            var entity = new AuditType { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
