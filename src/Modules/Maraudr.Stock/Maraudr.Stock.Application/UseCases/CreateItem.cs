namespace Maraudr.Stock.Application.UseCases;

public interface ICreateItemHandler
{
    Task<Guid> HandleAsync(CreateItemCommand command);
}

public class CreateItemHandler(IStockRepository repository) : ICreateItemHandler
{
    public async Task<Guid> HandleAsync(CreateItemCommand command)
    {
        if (!string.IsNullOrEmpty(command.BarCode))
        {
            var existingItem = await repository.GetStockItemByBarCodeAsync(command.BarCode);
        
            if (existingItem != null && existingItem.StockId == command.StockId)
            {
                await repository.AddQuantityToStock(existingItem.Id, 1);
                return existingItem.Id;
            }
        }

        var item = new StockItem(
            command.Name,
            command.Description,
            command.BarCode,
            command.ItemType,
            command.StockId
        );

        await repository.CreateStockItemAsync(item);
        return item.Id;
    }
}

