using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.Departments
{
    public class DepartmentEntityTests
    {
        [Fact]
        public void Department_ShouldBeCreatable()
        {
            // Act
            var entity = new Department();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Department_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Department();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsActive.Should().Be(false);
        }

        [Fact]
        public void Department_Code_ShouldAcceptValue()
        {
            var entity = new Department { Code = "Test Value" };
            entity.Code.Should().Be("Test Value");
        }

        [Fact]
        public void Department_Name_ShouldAcceptValue()
        {
            var entity = new Department { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
