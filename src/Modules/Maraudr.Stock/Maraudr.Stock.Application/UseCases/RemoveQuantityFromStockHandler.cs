namespace Maraudr.Stock.Application.UseCases;

public interface IRemoveQuantityFromStockHandler
{
    public Task HandleAsync(Guid id, int quantity);
}

public class RemoveQuantityFromStockHandler(IStockRepository repository) : IRemoveQuantityFromStockHandler
{
    public async Task HandleAsync(Guid id, int quantity)
    {
        await repository.RemoveQuantityFromStock(id, quantity);

       
    }
}