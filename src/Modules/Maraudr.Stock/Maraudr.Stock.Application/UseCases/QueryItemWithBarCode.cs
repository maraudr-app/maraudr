namespace Maraudr.Stock.Application.UseCases;


public interface IQueryItemWithBarCodeHandler
{
    Task<StockItemQuery?> HandleAsync(string code,Guid associationId);
}

public class QueryItemWithBarCodeHandler(IStockRepository respository) : IQueryItemWithBarCodeHandler
{
    public async Task<StockItemQuery?> HandleAsync(string code,Guid associationId)
    {
        var stock = await respository.GetStockByIdAsync(associationId);
        if (stock == null)
        {
            throw new ArgumentException("Association non energistrée");
        }
        var existingItem = await respository.GetStockItemByBarCodeAsync(code,stock.Id);
        if (existingItem == null)
        {
            throw new ArgumentException("Item non energistrée");
        }
        return existingItem is null ? null : new StockItemQuery(existingItem.Id, existingItem.Name, existingItem.Description,existingItem.Quantity, existingItem.Category, existingItem.EntryDate);
    }
}