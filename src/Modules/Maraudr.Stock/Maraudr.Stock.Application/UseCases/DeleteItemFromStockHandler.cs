namespace Maraudr.Stock.Application.UseCases;

public interface IDeleteItemFromStockHandler
{
    Task<bool> HandleAsync(Guid associationId, Guid itemId);
}

public class DeleteItemFromStockHandler(IStockRepository repository) : IDeleteItemFromStockHandler
{
    public async Task<bool> HandleAsync(Guid associationId, Guid itemId)
    {
        var item = await repository.GetStockItemByIdAsync(itemId);
        if (item == null)
            return false;

        var stock = await repository.GetStockByIdAsync(item.StockId);
        if (stock == null || stock.AssociationId != associationId)
            return false;

        await repository.DeleteStockItemAsync(itemId);
        return true;
    }
}
