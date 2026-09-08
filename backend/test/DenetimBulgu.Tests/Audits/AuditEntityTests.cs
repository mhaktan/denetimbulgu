using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.Audits
{
    public class AuditEntityTests
    {
        [Fact]
        public void Audit_ShouldBeCreatable()
        {
            // Act
            var entity = new Audit();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Audit_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Audit();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Audit_AuditNumber_ShouldAcceptValue()
        {
            var entity = new Audit { AuditNumber = "Test Value" };
            entity.AuditNumber.Should().Be("Test Value");
        }

    }
}
