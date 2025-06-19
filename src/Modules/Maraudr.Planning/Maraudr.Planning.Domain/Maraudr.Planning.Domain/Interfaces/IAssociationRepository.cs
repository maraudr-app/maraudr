namespace Maraudr.Planning.Domain.Interfaces;

public interface IAssociationRepository
{

    public  Task<bool> ExistsByIdAsync(Guid id);
}