namespace Maraudr.User.Domain.ValueObjects.Users;

public class Disponibility
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public Guid AssociationId { get; set; } // Champ obligatoire pour l'association
    
    public Disponibility() { }

    public Disponibility(Guid userId, DateTime start, DateTime end, Guid associationId)
    {
        if (start >= end)
            throw new ArgumentException("La date de début doit être antérieure à la date de fin");

        Id = Guid.NewGuid();
        Start = start;
        End = end;
        UserId = userId;
        AssociationId = associationId;
    }
    public Disponibility(Guid id,Guid userId, DateTime start, DateTime end, Guid associationId)
    {
        if (start >= end)
            throw new ArgumentException("La date de début doit être antérieure à la date de fin");

        Id = id;
        Start = start;
        End = end;
        UserId = userId;
        AssociationId = associationId;
    }
    

    public bool Overlaps(Disponibility other)
    {
        return AssociationId == other.AssociationId && Start < other.End && End > other.Start;
    }

    public void UpdateDates(DateTime start, DateTime end)
    {
        if (start >= end)
            throw new ArgumentException("La date de début doit être antérieure à la date de fin");
        if (Start < end && End > start)
        {
            throw new ArgumentException("Nouvelle disponibilité chevauche une disponibilité déjà présente");

        }
        Start = start;
        End = end;
    }
}
