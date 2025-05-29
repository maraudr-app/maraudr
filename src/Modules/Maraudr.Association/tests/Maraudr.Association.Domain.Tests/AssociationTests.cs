using Maraudr.Associations.Domain.Siret;
using Maraudr.Associations.Domain.ValueObjects;

namespace Maraudr.Association.Domain.Tests;

public class AssociationTests
{
    [Fact]
    public void Association_WithValidSiretAndAddress_ShouldHave_OneSiretAndAddress()
    {
        var siret = new SiretNumber("73282932000074");
        var address = new Address("56 rue Myrha", "Paris", "75018", "France");

        var association = new Associations.Domain.Entities.Association("Maraudr", "Paris", "FR", siret, address);

        Assert.Equal("73282932000074", association.Siret?.Value);
        Assert.NotNull(association.Siret);
        Assert.Equal(address, association.Address);
        Assert.Equal("Paris", association.City);
        Assert.Equal("FR", association.Country);
    }

    [Fact]
    public void Association_WithInvalidSiret_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SiretNumber("12345678901234"));
    }

    [Fact]
    public void Association_ShouldHave_UniqueGuid()
    {
        var address = new Address("56 rue Myrha", "Paris", "75018", "France");
        var association = new Associations.Domain.Entities.Association("Maraudr", "Paris", "FR", new SiretNumber("73282932000074"), address);
        var association2 = new Associations.Domain.Entities.Association("Maraudr", "Paris", "FR", new SiretNumber("73282932000074"), address);

        Assert.NotEqual(association.Id, association2.Id);
        Assert.False(association.Equals(association2));
    }
}