using Maraudr.Planning.Domain.Interfaces;

namespace Maraudr.Planning.Application.UseCases;

public class CreatePlanning
{
    
}

public interface ICreatePlanningHandler
{
    Task<Guid> HandleAsync(Guid associationId);
}
public class CreatePlanningHandler(IPlanningRepository repository) : ICreatePlanningHandler
{
    public async Task<Guid> HandleAsync(Guid associationId)
    {
        var planning = new Domain.Entities.Planning(associationId);
        await repository.CreatePlanningAsync(planning);
        return planning.Id;
    }
}
