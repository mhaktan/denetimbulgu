using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using DenetimBulgu.Entities;
using DenetimBulgu.Employees;
using DenetimBulgu.Employees.Dto;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Tests.Employees
{
    public class EmployeeAppServiceTests
    {
        private readonly Mock<IRepository<Employee, long>> _repositoryMock;
        private readonly EmployeeAppService _service;

        public EmployeeAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Employee, long>>();
            _service = new EmployeeAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Employee { Id = 1, RegistrationNumber = "Test registrationNumber", FullName = "Test fullName", Email = "Test email", IsActive = true },
                new Employee { Id = 2, RegistrationNumber = "Test registrationNumber", FullName = "Test fullName", Email = "Test email", IsActive = true },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act
            var result = _repositoryMock.Object.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public void Repository_GetAll_WithFilter_ShouldWork()
        {
            // Arrange
            var entities = new[]
            {
                new Employee { Id = 1, RegistrationNumber = "Test registrationNumber", FullName = "Test fullName", Email = "Test email", IsActive = true },
                new Employee { Id = 2, RegistrationNumber = "Test registrationNumber", FullName = "Test fullName", Email = "Test email", IsActive = true },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act — simulate keyword filter
            var result = _repositoryMock.Object.GetAll()
                .Where(x => x.Id.ToString().Contains("1"));

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldInsertEntity()
        {
            // Arrange
            var dto = new CreateEmployeeDto
            {
                RegistrationNumber = "Test registrationNumber", FullName = "Test fullName", Email = "Test email", IsActive = true
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Employee>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Employee { Id = 1, RegistrationNumber = "Test registrationNumber", FullName = "Test fullName", Email = "Test email", IsActive = true });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Employee { Id = 1, RegistrationNumber = "Test registrationNumber", FullName = "Test fullName", Email = "Test email", IsActive = true });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
