namespace Maraudr.Stock.Application.UseCases;

public interface ICreateItemFromBarcodeHandler
{
    public Task<Guid> HandleAsync(string barcode, Guid assocationId);
}
public class CreateItemFromBarcodeHandler(
    IStockRepository repository,
    IOpenFoodFactRepository offRepository
) : ICreateItemFromBarcodeHandler
{
    public async Task<Guid> HandleAsync(string barcode, Guid associationId)
    {
        var stock = await repository.GetStockByAssociationIdAsync(associationId);
        if (stock is null)
        {
            throw new ArgumentException("Stock introuvable pour l'association");
        }

        Console.WriteLine(stock.Id);

        var existingItem = await repository.GetStockItemByBarCodeAndStockIdAsync(barcode, stock.Id);
        if (existingItem != null)
        {
            await repository.AddQuantityToStock(existingItem.Id, 1);
            return existingItem.Id;
        }

        var itemFromOff = await offRepository.GetAllProductDataByCode(barcode);
        if (itemFromOff is null)
        {
            throw new ArgumentException("Produit inexistant dans OpenFoodFacts");
        }

        var item = new StockItem(
            itemFromOff.Name,
            itemFromOff.Description,
            itemFromOff.BarCode,
            itemFromOff.Category,
            stock.Id
        );

        await repository.CreateStockItemAsync(item);
        return item.Id;
    }
}
