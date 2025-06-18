using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maraudr.Planning.Application.DTOs;

namespace Maraudr.Planning.Application.UseCases
{
    public interface ICreateAnEventHandler
    {
        public Task<Guid> HandleAsync(Guid organizerId, CreateEventDto request);

   }

    public class CreateAnEventHandler(IPlanningRepository repository,IAssociationRepository associationRepository) : ICreateAnEventHandler
    {
        public async Task<Guid> HandleAsync(Guid organizerId, CreateEventDto request)
        {
            var association = await associationRepository.GetByIdAsync(request.AssociationId);
            var planningId = repository.GetPlanningIdFromAssociation(request.AssociationId);
            if (association == null)
                throw new InvalidOperationException("Association not found");
            var @event = new Event
            {
                PlanningId = planningId,
                OrganizerId = organizerId,
                ParticipantsIds = request.ParticipantsIds,
                BeginningDate = request.BeginningDate,
                EndDate = request.EndDate,
                Title = request.Title,
                Description = request.Description,
                Street = request.Street,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country
            };
            await repository.AddAsync(@event);
            return @event.Id;
        }
    }
}
