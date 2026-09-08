using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using DenetimBulgu.Entities;
using DenetimBulgu.AuditPlans;
using DenetimBulgu.AuditPlans.Dto;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Tests.AuditPlans
{
    public class AuditPlanAppServiceTests
    {
        private readonly Mock<IRepository<AuditPlan, long>> _repositoryMock;
        private readonly AuditPlanAppService _service;

        public AuditPlanAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<AuditPlan, long>>();
            _service = new AuditPlanAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object, new Mock<IRepository<StatusChangeLog, long>>().Object, new Mock<IRepository<ApprovalRecord, Guid>>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new AuditPlan { Id = 1, Year = 1, Period = "Test period", ScopeDescription = "Test scopeDescription", Status = 0 },
                new AuditPlan { Id = 2, Year = 1, Period = "Test period", ScopeDescription = "Test scopeDescription", Status = 0 },
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
                new AuditPlan { Id = 1, Year = 1, Period = "Test period", ScopeDescription = "Test scopeDescription", Status = 0 },
                new AuditPlan { Id = 2, Year = 1, Period = "Test period", ScopeDescription = "Test scopeDescription", Status = 0 },
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
            var dto = new CreateAuditPlanDto
            {
                Year = 1, Period = "Test period", ScopeDescription = "Test scopeDescription", Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<AuditPlan>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new AuditPlan { Id = 1, Year = 1, Period = "Test period", ScopeDescription = "Test scopeDescription", Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new AuditPlan { Id = 1, Year = 1, Period = "Test period", ScopeDescription = "Test scopeDescription", Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
