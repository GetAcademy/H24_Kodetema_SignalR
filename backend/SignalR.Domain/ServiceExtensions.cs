using Microsoft.Extensions.DependencyInjection;
using SignalR.Domain.Database;
using SignalR.Domain.Repositories;

namespace SignalR.Domain;

public static class ServiceExtensions
{
    public static IServiceCollection AddTodoDatabase(this IServiceCollection services)
    {
        services.AddSingleton<ITodoDbContextGenerator, InMemoryDbContextGenerator>();
        services.AddTransient<IDocumentRepository, DocumentRepository>();

        return services;
    }
}