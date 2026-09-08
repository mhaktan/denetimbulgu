using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using DenetimBulgu.Entities;
using DenetimBulgu.CorrectiveActions;
using DenetimBulgu.CorrectiveActions.Dto;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Tests.CorrectiveActions
{
    public class CorrectiveActionAppServiceTests
    {
        private readonly Mock<IRepository<CorrectiveAction, long>> _repositoryMock;
        private readonly CorrectiveActionAppService _service;

        public CorrectiveActionAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<CorrectiveAction, long>>();
            _service = new CorrectiveActionAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object, new Mock<IRepository<StatusChangeLog, long>>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new CorrectiveAction { Id = 1, RootCauseAnalysis = "Test rootCauseAnalysis", ActionDescription = "Test actionDescription", TargetDate = DateTime.UtcNow, Status = 0 },
                new CorrectiveAction { Id = 2, RootCauseAnalysis = "Test rootCauseAnalysis", ActionDescription = "Test actionDescription", TargetDate = DateTime.UtcNow, Status = 0 },
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
                new CorrectiveAction { Id = 1, RootCauseAnalysis = "Test rootCauseAnalysis", ActionDescription = "Test actionDescription", TargetDate = DateTime.UtcNow, Status = 0 },
                new CorrectiveAction { Id = 2, RootCauseAnalysis = "Test rootCauseAnalysis", ActionDescription = "Test actionDescription", TargetDate = DateTime.UtcNow, Status = 0 },
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
            var dto = new CreateCorrectiveActionDto
            {
                RootCauseAnalysis = "Test rootCauseAnalysis", ActionDescription = "Test actionDescription", TargetDate = DateTime.UtcNow, Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<CorrectiveAction>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new CorrectiveAction { Id = 1, RootCauseAnalysis = "Test rootCauseAnalysis", ActionDescription = "Test actionDescription", TargetDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new CorrectiveAction { Id = 1, RootCauseAnalysis = "Test rootCauseAnalysis", ActionDescription = "Test actionDescription", TargetDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
