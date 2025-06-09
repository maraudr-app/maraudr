using Application.DTOs.DisponibilitiesQueriesDtos.Requests;

namespace Application.UseCases.Disponibilities.UpdateDisponibility;

public interface IUpdateDisponibilityHandler
{
    public Task HandleAsync(Guid dispoId,Guid id,UpdateDisponiblityRequest request);

}