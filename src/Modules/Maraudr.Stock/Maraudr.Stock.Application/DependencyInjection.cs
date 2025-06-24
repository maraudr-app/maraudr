namespace Maraudr.Stock.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateItemHandler, CreateItemHandler>();
        services.AddScoped<IQueryItemByType, QueryItemByType>();
        services.AddScoped<IQueryItemHandler, QueryItemHandler>();
        services.AddScoped<IDeleteItemHandler, DeleteItemHandler>();
        services.AddScoped<ICreateItemFromBarcodeHandler, CreateItemFromBarcodeHandler>();
        services.AddScoped<IRemoveQuantityFromStockHandler, RemoveQuantityFromStockHandler>();
        services.AddScoped<IQueryItemWithBarCodeHandler, QueryItemWithBarCodeHandler>();
        services.AddScoped<ICreateStockHandler, CreateStock>();
        services.AddScoped<IGetItemsOfAssociationHandler, GetItemsOfAssociationHandler>();
        services.AddScoped<IDeleteItemFromStockHandler, DeleteItemFromStockHandler>();
        services.AddScoped<IGetStockIdByAssociationHandler, GetStockIdByAssociationHandler>();
        services.AddScoped<IQueryItemByAssociationHandler, QueryItemByAssociationHandler>();
        services.AddScoped<IReduceQuantityFromItemHandler, ReduceQuantityFromItemHandler>();
    }
}
