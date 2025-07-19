
using Maraudr.Planning.Application.DTOs;
using Maraudr.Planning.Domain.Entities;
using Maraudr.Planning.Domain.Interfaces;

namespace Maraudr.Planning.Application.UseCases
{
    public interface ICreateAnEventHandler
    {
        public Task<Guid> HandleAsync(Guid organizerId, CreateEventDto request);

   }

    public class CreateAnEventHandler(IPlanningRepository repository,IEmailingRepository emailingRepository ) : ICreateAnEventHandler
    {
        public async Task<Guid> HandleAsync(Guid organizerId, CreateEventDto request)
        {
            var exists = await repository.AssociationExistsByIdAsync(request.AssociationId);
            if (!exists)
                throw new InvalidOperationException("Association not found");
            
            var planningId = await repository.GetPlanningIdFromAssociationAsync(request.AssociationId);

            var @event = new Event
            {
                PlanningId = planningId,
                OrganizerdId = organizerId,
                ParticipantsIds = request.ParticipantsIds,
                BeginningDate = request.BeginningDate,
                EndDate = request.EndDate,
                Title = request.Title,
                Description = request.Description,
                Location = request.Location
            };
            
            await repository.AddEventAsync(@event);
            await emailingRepository.SendEventEmailAsync(request.ParticipantsIds,request.Title,request.Description);
            return @event.Id;
        }
    }
}
