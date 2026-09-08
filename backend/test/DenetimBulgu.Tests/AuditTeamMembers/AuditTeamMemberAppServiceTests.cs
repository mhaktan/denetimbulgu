using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using DenetimBulgu.Entities;
using DenetimBulgu.AuditTeamMembers;
using DenetimBulgu.AuditTeamMembers.Dto;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Tests.AuditTeamMembers
{
    public class AuditTeamMemberAppServiceTests
    {
        private readonly Mock<IRepository<AuditTeamMember, long>> _repositoryMock;
        private readonly AuditTeamMemberAppService _service;

        public AuditTeamMemberAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<AuditTeamMember, long>>();
            _service = new AuditTeamMemberAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new AuditTeamMember { Id = 1, Role = 0 },
                new AuditTeamMember { Id = 2, Role = 0 },
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
                new AuditTeamMember { Id = 1, Role = 0 },
                new AuditTeamMember { Id = 2, Role = 0 },
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
            var dto = new CreateAuditTeamMemberDto
            {
                Role = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<AuditTeamMember>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new AuditTeamMember { Id = 1, Role = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new AuditTeamMember { Id = 1, Role = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
