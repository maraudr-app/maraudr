namespace Maraudr.Stock.Endpoints;

public static class DependencyInjection
{
    public static void AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateItemCommand>, ItemValidator>();
    }
}
