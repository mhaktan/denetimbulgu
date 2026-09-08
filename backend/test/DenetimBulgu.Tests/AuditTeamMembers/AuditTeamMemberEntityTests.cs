using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.AuditTeamMembers
{
    public class AuditTeamMemberEntityTests
    {
        [Fact]
        public void AuditTeamMember_ShouldBeCreatable()
        {
            // Act
            var entity = new AuditTeamMember();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void AuditTeamMember_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new AuditTeamMember();

            // Assert
            entity.Id.Should().Be(default(long));

        }


    }
}
