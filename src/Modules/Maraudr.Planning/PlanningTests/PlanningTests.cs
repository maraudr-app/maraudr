namespace PlanningTests;

using Xunit;
using FluentAssertions;
using Maraudr.Planning.Domain.Entities;


    public class PlanningTests
    {
        [Fact]
        public void Constructor_WithAssociationId_ShouldCreatePlanning()
        {
            // Arrange
            var associationId = Guid.NewGuid();

            // Act
            var planning = new Planning(associationId);

            // Assert
            planning.Id.Should().NotBeEmpty();
            planning.AssociationId.Should().Be(associationId);
            planning.Events.Should().BeEmpty();
        }

        [Fact]
        public void RemoveEvent_WithExistingEvent_ShouldRemoveEvent()
        {
            // Arrange
            var planning = new Planning(Guid.NewGuid());
            var eventId = Guid.NewGuid();
            var eventObj = CreateTestEvent();
            eventObj.Id = eventId;
            planning.Events.Add(eventObj);

            // Act
            planning.RemoveEvent(eventId);

            // Assert
            planning.Events.Should().NotContain(e => e.Id == eventId);
        }

        private Event CreateTestEvent()
        {
            return new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new List<Guid>(),
                "Test Event",
                "Description",
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(1).AddHours(2),
                "Location"
            );
        }
    }
