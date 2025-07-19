namespace Application.UseCases.Disponibilities.DeleteDisponiblity;

public interface IDeleteDisponibilityHandler
{
    public Task HandleAsync(Guid id,Guid dispoId);
}