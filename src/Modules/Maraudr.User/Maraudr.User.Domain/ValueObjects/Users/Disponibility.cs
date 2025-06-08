namespace Maraudr.User.Domain.ValueObjects.Users;

public class Disponibility
{
        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get;  set; }

        
        public Disponibility() { }

        public Disponibility(Guid userId, DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("La date de début doit être antérieure à la date de fin");

            Id = Guid.NewGuid();
            UserId = userId;
            Start = start;
            End = end;
        }

       

        public bool Overlaps(Disponibility other)
        {
            return Start < other.End && End > other.Start;
        }
    
}