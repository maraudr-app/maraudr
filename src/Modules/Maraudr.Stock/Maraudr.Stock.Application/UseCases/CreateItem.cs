namespace Maraudr.Stock.Application.UseCases;

public interface ICreateItemHandler
{
    Task<Guid> HandleAsync(CreateItemCommand command);
}

public class CreateItemHandler(IStockRepository repository) : ICreateItemHandler
{
    public async Task<Guid> HandleAsync(CreateItemCommand command)
    {
        var savedItem = await repository.GetStockItemByBarCodeAsync(command.BarCode);
        if (savedItem != null)
        {
            await repository.AddQuantityToStock(savedItem.Id, 1);
            return savedItem.Id;
        }
        var item = new StockItem(command.Name, command.Description,command.BarCode, command.ItemType);
        await repository.CreateStockItemAsync(item);
        return item.Id;
    }
}
