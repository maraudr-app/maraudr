namespace Maraudr.Stock.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateItemHandler, CreateItemHandler>();
        services.AddScoped<IQueryItemByType, QueryItemByType>();
        services.AddScoped<IQueryItemHandler, QueryItemHandler>();
        services.AddScoped<IDeleteItemHandler, DeleteItemHandler>();
        services.AddScoped<ICreateMultipleItemsHandler, CreateMultipleItems>();
    }
}
