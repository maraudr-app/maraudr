namespace GeoTest;

using Xunit;
using FluentAssertions;
using Maraudr.Geo.Domain.Entities;


    public class ItineraryTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange
            var associationId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var startLat = 48.8566;
            var startLng = 2.3522;
            var centerLat = 48.8584;
            var centerLng = 2.2945;
            var radiusKm = 5.0;
            var geoJson = "{}";
            var gmapsUrl = "https://maps.google.com";
            var distanceKm = 10.5;
            var durationMinutes = 25.0;

            // Act
            var itinerary = new Itinerary(associationId, eventId, startLat, startLng, centerLat, centerLng, radiusKm, geoJson, gmapsUrl, distanceKm, durationMinutes);

            // Assert
            itinerary.AssociationId.Should().Be(associationId);
            itinerary.EventId.Should().Be(eventId);
            itinerary.StartLat.Should().Be(startLat);
            itinerary.StartLng.Should().Be(startLng);
            itinerary.IsActive.Should().BeTrue();
            itinerary.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }