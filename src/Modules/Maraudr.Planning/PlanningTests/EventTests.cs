namespace PlanningTests;

using Xunit;
using FluentAssertions;
using Maraudr.Planning.Domain.Entities;
using Maraudr.Planning.Domain.ValueObjects;


    public class EventTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateEvent()
        {
            // Arrange
            var planningId = Guid.NewGuid();
            var organizerId = Guid.NewGuid();
            var participantsIds = new List<Guid> { Guid.NewGuid() };
            var title = "Test Event";
            var description = "Test Description";
            var beginningDate = DateTime.Now.AddDays(1);
            var endDate = DateTime.Now.AddDays(1).AddHours(2);
            var location = "Test Location";

            // Act
            var eventObj = new Event(planningId, organizerId, participantsIds, title, description, beginningDate, endDate, location);

            // Assert
            eventObj.Id.Should().NotBeEmpty();
            eventObj.Status.Should().Be(Status.CREATED);
            eventObj.Title.Should().Be(title);
        }

        [Fact]
        public void AddParticipant_WithNewParticipant_ShouldAddSuccessfully()
        {
            // Arrange
            var eventObj = CreateTestEvent();
            var newParticipantId = Guid.NewGuid();

            // Act
            eventObj.AddParticipant(newParticipantId);

            // Assert
            eventObj.ParticipantsIds.Should().Contain(newParticipantId);
        }

        [Fact]
        public void AddParticipant_WithExistingParticipant_ShouldThrowArgumentException()
        {
            // Arrange
            var participantId = Guid.NewGuid();
            var eventObj = CreateTestEvent(new List<Guid> { participantId });

            // Act & Assert
            Assert.Throws<ArgumentException>(() => eventObj.AddParticipant(participantId));
        }

        [Fact]
        public void ChangeStatus_FromCreatedToOngoing_ShouldSucceed()
        {
            // Arrange
            var eventObj = CreateTestEvent();

            // Act
            eventObj.ChangeStatus(Status.ONGOING);

            // Assert
            eventObj.Status.Should().Be(Status.ONGOING);
        }

        [Fact]
        public void ChangeStatus_FromCanceled_ShouldThrowArgumentException()
        {
            // Arrange
            var eventObj = CreateTestEvent();
            eventObj.ChangeStatus(Status.CANCELED);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => eventObj.ChangeStatus(Status.ONGOING));
        }

        private Event CreateTestEvent(List<Guid>? participantsIds = null)
        {
            return new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                participantsIds ?? new List<Guid>(),
                "Test Event",
                "Description",
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(1).AddHours(2),
                "Location"
            );
        }
    }