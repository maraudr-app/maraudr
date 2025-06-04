namespace Maraudr.Stock.Application.UseCases;


public interface IQueryItemWithBarCodeHandler
{
    Task<StockItemQuery?> HandleAsync(string code);
}

public class QueryItemWithBarCodeHandler(IStockRepository respository) : IQueryItemWithBarCodeHandler
{
    public async Task<StockItemQuery?> HandleAsync(string code)
    {
        var item = await respository.GetStockItemByBarCodeAsync(code);
        return item is null ? null : new StockItemQuery(item.Id, item.Name, item.Description,item.Quantity, item.Category, item.EntryDate);
    }
}