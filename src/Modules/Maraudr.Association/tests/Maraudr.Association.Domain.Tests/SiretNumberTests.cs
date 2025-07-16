using Xunit;
using FluentAssertions;
using Maraudr.Associations.Domain.Siret;
namespace Maraudr.Association.Domain.Tests;


    public class SiretNumberTests
    {
        
        [Theory]
        [InlineData("123")] // Trop court
        [InlineData("123456789012345")] // Trop long
        [InlineData("1234567890123a")] // Contient des lettres
        [InlineData("12345678901235")] // Échec du contrôle de Luhn
        public void Constructor_WithInvalidSiret_ShouldThrowArgumentException(string invalidSiret)
        {
            Assert.Throws<ArgumentException>(() => new SiretNumber(invalidSiret));
        }

        [Fact]
        public void Equals_WithSameSiret_ShouldReturnTrue()
        {
            // Arrange
            var siret1 = new SiretNumber("73282932000074");
            var siret2 = new SiretNumber("73282932000074");

            // Act & Assert
            siret1.Equals(siret2).Should().BeTrue();
        }

        [Fact]
        public void GetHashCode_WithSameSiret_ShouldReturnSameHashCode()
        {
            // Arrange
            var siret1 = new SiretNumber("73282932000074");
            var siret2 = new SiretNumber("73282932000074");

            // Act & Assert
            siret1.GetHashCode().Should().Be(siret2.GetHashCode());
        }
    }
