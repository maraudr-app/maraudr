namespace Maraudr.Planning.Domain.Entities;

public class Planning
{
    public Guid Id { get; init; } 
    public Guid AssociationId { get; set; }
    public List<Event> Events { get; set; }


    public Planning()
    {
        
    }

    public Planning(Guid associationId)
    {
        Id = Guid.NewGuid();
        AssociationId = associationId;
        Events = [];
    }
    
    public void AddEvent(Event eventTarget)
    {
        if (!Overlap(eventTarget))
        {
            Events.Add(eventTarget);
        }

        throw new ArgumentException("Event overlaps another event");
    }

    public void RemoveEvent(Guid eventId)
    {
        Events.Where(ev => ev.Id == eventId).ToList().ForEach(ev => Events.Remove(ev));  
    }

    public bool Overlap(Event eventTarget){
        foreach (Event l_event in Events )
        {
            if (eventTarget.Overlaps(l_event))
            {
                return true;
            }
        }
        return false;
    }
   
}