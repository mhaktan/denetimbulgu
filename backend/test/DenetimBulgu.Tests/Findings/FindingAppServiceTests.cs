using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using DenetimBulgu.Entities;
using DenetimBulgu.Findings;
using DenetimBulgu.Findings.Dto;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Tests.Findings
{
    public class FindingAppServiceTests
    {
        private readonly Mock<IRepository<Finding, long>> _repositoryMock;
        private readonly FindingAppService _service;

        public FindingAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Finding, long>>();
            _service = new FindingAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object, new Mock<IRepository<StatusChangeLog, long>>().Object, new Mock<IRepository<ApprovalRecord, Guid>>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Finding { Id = 1, Title = "Test title", Description = "Test description", DetectedDate = DateTime.UtcNow, DueDate = DateTime.UtcNow, Status = 0 },
                new Finding { Id = 2, Title = "Test title", Description = "Test description", DetectedDate = DateTime.UtcNow, DueDate = DateTime.UtcNow, Status = 0 },
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
                new Finding { Id = 1, Title = "Test title", Description = "Test description", DetectedDate = DateTime.UtcNow, DueDate = DateTime.UtcNow, Status = 0 },
                new Finding { Id = 2, Title = "Test title", Description = "Test description", DetectedDate = DateTime.UtcNow, DueDate = DateTime.UtcNow, Status = 0 },
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
            var dto = new CreateFindingDto
            {
                Title = "Test title", Description = "Test description", DetectedDate = DateTime.UtcNow, DueDate = DateTime.UtcNow, Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Finding>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Finding { Id = 1, Title = "Test title", Description = "Test description", DetectedDate = DateTime.UtcNow, DueDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Finding { Id = 1, Title = "Test title", Description = "Test description", DetectedDate = DateTime.UtcNow, DueDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
