namespace Maraudr.Stock.Application.UseCases;

public interface IGetStockIdByAssociationHandler
{
    Task<Guid?> HandleAsync(Guid associationId);
}

public class GetStockIdByAssociationHandler(IStockRepository repository) : IGetStockIdByAssociationHandler
{
    public async Task<Guid?> HandleAsync(Guid associationId)
    {
        return await repository.GetStockIdByAssociationIdAsync(associationId);
    }
}
