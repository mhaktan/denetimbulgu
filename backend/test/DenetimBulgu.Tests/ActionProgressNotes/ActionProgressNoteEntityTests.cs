using System;
using Xunit;
using FluentAssertions;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Tests.ActionProgressNotes
{
    public class ActionProgressNoteEntityTests
    {
        [Fact]
        public void ActionProgressNote_ShouldBeCreatable()
        {
            // Act
            var entity = new ActionProgressNote();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void ActionProgressNote_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new ActionProgressNote();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void ActionProgressNote_Note_ShouldAcceptValue()
        {
            var entity = new ActionProgressNote { Note = "Test Value" };
            entity.Note.Should().Be("Test Value");
        }

    }
}
