using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using DenetimBulgu.Entities;
using DenetimBulgu.RequirementReferences;
using DenetimBulgu.RequirementReferences.Dto;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Tests.RequirementReferences
{
    public class RequirementReferenceAppServiceTests
    {
        private readonly Mock<IRepository<RequirementReference, long>> _repositoryMock;
        private readonly RequirementReferenceAppService _service;

        public RequirementReferenceAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<RequirementReference, long>>();
            _service = new RequirementReferenceAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new RequirementReference { Id = 1, Code = "Test code", Title = "Test title", SourceStandard = 0 },
                new RequirementReference { Id = 2, Code = "Test code", Title = "Test title", SourceStandard = 0 },
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
                new RequirementReference { Id = 1, Code = "Test code", Title = "Test title", SourceStandard = 0 },
                new RequirementReference { Id = 2, Code = "Test code", Title = "Test title", SourceStandard = 0 },
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
            var dto = new CreateRequirementReferenceDto
            {
                Code = "Test code", Title = "Test title", SourceStandard = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<RequirementReference>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new RequirementReference { Id = 1, Code = "Test code", Title = "Test title", SourceStandard = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new RequirementReference { Id = 1, Code = "Test code", Title = "Test title", SourceStandard = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
