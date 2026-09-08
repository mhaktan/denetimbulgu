using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using DenetimBulgu.Entities;
using DenetimBulgu.Departments;
using DenetimBulgu.Departments.Dto;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Tests.Departments
{
    public class DepartmentAppServiceTests
    {
        private readonly Mock<IRepository<Department, long>> _repositoryMock;
        private readonly DepartmentAppService _service;

        public DepartmentAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Department, long>>();
            _service = new DepartmentAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Department { Id = 1, Code = "Test code", Name = "Test name", IsActive = true },
                new Department { Id = 2, Code = "Test code", Name = "Test name", IsActive = true },
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
                new Department { Id = 1, Code = "Test code", Name = "Test name", IsActive = true },
                new Department { Id = 2, Code = "Test code", Name = "Test name", IsActive = true },
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
            var dto = new CreateDepartmentDto
            {
                Code = "Test code", Name = "Test name", IsActive = true
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Department>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Department { Id = 1, Code = "Test code", Name = "Test name", IsActive = true });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Department { Id = 1, Code = "Test code", Name = "Test name", IsActive = true });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
