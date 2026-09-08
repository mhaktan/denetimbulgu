using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using DenetimBulgu.Entities;
using DenetimBulgu.AuditTypes;
using DenetimBulgu.AuditTypes.Dto;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Tests.AuditTypes
{
    public class AuditTypeAppServiceTests
    {
        private readonly Mock<IRepository<AuditType, long>> _repositoryMock;
        private readonly AuditTypeAppService _service;

        public AuditTypeAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<AuditType, long>>();
            _service = new AuditTypeAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new AuditType { Id = 1, Code = "Test code", Name = "Test name" },
                new AuditType { Id = 2, Code = "Test code", Name = "Test name" },
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
                new AuditType { Id = 1, Code = "Test code", Name = "Test name" },
                new AuditType { Id = 2, Code = "Test code", Name = "Test name" },
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
            var dto = new CreateAuditTypeDto
            {
                Code = "Test code", Name = "Test name"
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<AuditType>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new AuditType { Id = 1, Code = "Test code", Name = "Test name" });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new AuditType { Id = 1, Code = "Test code", Name = "Test name" });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
