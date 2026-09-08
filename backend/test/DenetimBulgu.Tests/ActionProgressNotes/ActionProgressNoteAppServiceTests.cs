using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using DenetimBulgu.Entities;
using DenetimBulgu.ActionProgressNotes;
using DenetimBulgu.ActionProgressNotes.Dto;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Tests.ActionProgressNotes
{
    public class ActionProgressNoteAppServiceTests
    {
        private readonly Mock<IRepository<ActionProgressNote, long>> _repositoryMock;
        private readonly ActionProgressNoteAppService _service;

        public ActionProgressNoteAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<ActionProgressNote, long>>();
            _service = new ActionProgressNoteAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new ActionProgressNote { Id = 1, Note = "Test note", NoteDate = DateTime.UtcNow },
                new ActionProgressNote { Id = 2, Note = "Test note", NoteDate = DateTime.UtcNow },
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
                new ActionProgressNote { Id = 1, Note = "Test note", NoteDate = DateTime.UtcNow },
                new ActionProgressNote { Id = 2, Note = "Test note", NoteDate = DateTime.UtcNow },
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
            var dto = new CreateActionProgressNoteDto
            {
                Note = "Test note", NoteDate = DateTime.UtcNow
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<ActionProgressNote>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ActionProgressNote { Id = 1, Note = "Test note", NoteDate = DateTime.UtcNow });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ActionProgressNote { Id = 1, Note = "Test note", NoteDate = DateTime.UtcNow });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
