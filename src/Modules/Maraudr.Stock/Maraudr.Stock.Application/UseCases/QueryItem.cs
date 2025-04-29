namespace Maraudr.Stock.Application.UseCases;

public record StockItemQuery(Guid Id, string Name, string? Description, Category Type, DateTime DateTime);

public interface IQueryItemHandler
{
    Task<StockItemQuery?> HandleAsync(Guid id);
}

public class QueryItemHandler(IStockRepository respository) : IQueryItemHandler
{
    public async Task<StockItemQuery?> HandleAsync(Guid id)
    {
        var item = await respository.GetStockItemByIdAsync(id);
        return item is null ? null : new StockItemQuery(item.Id, item.Name, item.Description, item.Category, item.EntryDate);
    }
}
