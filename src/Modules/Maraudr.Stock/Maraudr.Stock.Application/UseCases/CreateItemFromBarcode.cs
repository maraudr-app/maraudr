namespace Maraudr.Stock.Application.UseCases;


public interface ICreateItemFromBarcodeHandler
{
    public Task<Guid> HandleAsync(string barcode);
}
public class CreateItemFromBarcodeHandler(IStockRepository repository, IOpenFoodFactRepository offRepository): ICreateItemFromBarcodeHandler
{
    public async Task<Guid> HandleAsync(string barcode)
    {
        var item = await offRepository.GetAllProductDataByCode(barcode);
        if (item == null)
        {
            throw new ArgumentException("Produit inexistant");
        }
        
        var savedItem = await repository.GetStockItemByBarCodeAsync(barcode);
        if (savedItem != null)
        {
            await repository.AddQuantityToStock(savedItem.Id, 1);
            return savedItem.Id;
        }
        
        await repository.CreateStockItemAsync(item);
        return item.Id;
            
        
        
    }
}