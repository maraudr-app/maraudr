
using System.Text.Json.Serialization;
using Maraudr.Planning.Domain.ValueObjects;

namespace Maraudr.Planning.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public Guid PlanningId { get; set; }
    public Guid OrganizerdId { get; set; }
    public List<Guid> ParticipantsIds { get; set; }
    public DateTime BeginningDate { get; set; }
    public DateTime EndDate { get; set; }
    
    /*[JsonConverter(typeof(JsonStringEnumConverter))]
    public Status Status { get; set; }*/
    
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }

    public Event()
    {

    }

    public Event(Guid planningId, Guid organizerdId, List<Guid> participantsIds, string title, string description,
        DateTime beginningDate, DateTime endDate, string location)
    {
        Id = Guid.NewGuid();
        PlanningId = planningId;
        OrganizerdId = organizerdId;
        ParticipantsIds = participantsIds;
        BeginningDate = beginningDate;
        EndDate = endDate;
        Title = title;
        Description = description;
        Location = location;
       // Status = Status.CREATED;
    }

    public void RemoveAParticipant(Guid participantId)

    {
        if (!ParticipantsIds.Contains(participantId))
        {
            throw new ArgumentException("User  not found");

        }

        ParticipantsIds.Remove(participantId);
    }

    public void AddParticipant(Guid participantId)
    {
        if (ParticipantsIds.Contains(participantId))
        {
            throw new ArgumentException("User already in");
        }
        
        ParticipantsIds.Add(participantId);
    }

    public bool Overlaps(Event other)
    {
        return PlanningId== other.PlanningId && BeginningDate < other.EndDate && EndDate > other.BeginningDate;

    }

    public override bool Equals(object? obj)
    {
        
        if (obj is not Event other)
            return false;

        return this.Id == other.Id && this.PlanningId == other.PlanningId;
        
    }
    
    
}