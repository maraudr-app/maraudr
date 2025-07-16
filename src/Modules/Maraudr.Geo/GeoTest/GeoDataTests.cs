using Xunit;
using FluentAssertions;
using Maraudr.Geo.Domain.Entities;

namespace GeoTest;

    public class GeoDataTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstanceWithDefaultValues()
        {
            // Act
            var geoData = new GeoData
            {
                GeoStoreId = Guid.NewGuid(),
                Latitude = 48.8566,
                Longitude = 2.3522,
                Notes = "Test location"
            };

            // Assert
            geoData.Id.Should().NotBeEmpty();
            geoData.IsActive.Should().BeTrue();
            geoData.ObservedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }
