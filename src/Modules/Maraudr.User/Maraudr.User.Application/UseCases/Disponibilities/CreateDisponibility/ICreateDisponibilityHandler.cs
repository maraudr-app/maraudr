using Application.DTOs.DisponibilitiesQueriesDtos.Requests;

namespace Application.UseCases.Disponibilities.CreateDisponibility;

public interface ICreateDisponibilityHandler
{
 public Task HandleAsync(Guid id,CreateDisponiblityRequest request);
}