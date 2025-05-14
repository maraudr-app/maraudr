using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Siret;

namespace Maraudr.Associations.Domain;

public abstract class AssociationCreator
{
    public abstract Association CreateAssociation();
}

public class BasicAssociationCreator(string name, string city, string country) 
    : AssociationCreator
{
    public override Association CreateAssociation()
    {
        return new Association(name, city, country);
    }
}

public class AssociationWithSiretCreator(string name, string city, string country, SiretNumber siret) 
    : AssociationCreator
{
    public override Association CreateAssociation()
    {
        return new Association(name, city, country, siret);
    }
}