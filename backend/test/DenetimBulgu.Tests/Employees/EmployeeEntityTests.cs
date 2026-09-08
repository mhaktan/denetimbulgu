using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.Employees
{
    public class EmployeeEntityTests
    {
        [Fact]
        public void Employee_ShouldBeCreatable()
        {
            // Act
            var entity = new Employee();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Employee_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Employee();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsActive.Should().Be(false);
        }

        [Fact]
        public void Employee_RegistrationNumber_ShouldAcceptValue()
        {
            var entity = new Employee { RegistrationNumber = "Test Value" };
            entity.RegistrationNumber.Should().Be("Test Value");
        }

        [Fact]
        public void Employee_FullName_ShouldAcceptValue()
        {
            var entity = new Employee { FullName = "Test Value" };
            entity.FullName.Should().Be("Test Value");
        }

        [Fact]
        public void Employee_Email_ShouldAcceptValue()
        {
            var entity = new Employee { Email = "Test Value" };
            entity.Email.Should().Be("Test Value");
        }

    }
}
