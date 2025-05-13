using Maraudr.Associations.Domain.Siret;

namespace Maraudr.Association.Domain.Tests;

public class AssociationTests
{
    [Fact]
    public void Association_WithValidSiret_ShouldHave_OneSiret()
    {
        var association = new Associations.Domain.Entities.Association("Maraudr", "Paris", "FR", new SiretNumber("73282932000074"));
        
        Assert.Equal("73282932000074", association.Siret?.Value);
        Assert.NotNull(association.Siret);
    }

    [Fact]
    public void Association_WithInvalidSiret_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SiretNumber("12345678901234"));
    }

    [Fact]
    public void Association_ShouldHave_UniqueGuid()
    {
        var association = new Associations.Domain.Entities.Association("Maraudr", "Paris", "FR");
        var association2 = new Associations.Domain.Entities.Association("Maraudr", "Paris", "FR");
        
        Assert.False(association.Equals(association2));
        Assert.NotEqual(association, association2);
    }
}