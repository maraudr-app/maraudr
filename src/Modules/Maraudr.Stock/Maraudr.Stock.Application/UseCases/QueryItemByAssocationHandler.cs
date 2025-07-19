namespace Maraudr.Stock.Application.UseCases;

public interface IQueryItemByAssociationHandler
{
    Task<StockItemQuery?> HandleAsync(Guid itemId, Guid associationId);
}

public class QueryItemByAssociationHandler(IStockRepository repository) : IQueryItemByAssociationHandler
{
    public async Task<StockItemQuery?> HandleAsync(Guid itemId, Guid associationId)
    {
        var item = await repository.GetStockItemByIdAsync(itemId);
        if (item is null)
            return null;

        var stock = await repository.GetStockByIdAsync(item.StockId);
        if (stock == null || stock.AssociationId != associationId)
            return null;

        return new StockItemQuery(
            item.Id,
            item.Name,
            item.Description,
            item.Quantity,
            item.Category,
            item.EntryDate
        );
    }
}
