using Host.Data;
using Host.Services;

namespace Host.Dependency;

public static class Dependencies
{
    public static void SetDependencies(IServiceCollection services)
    {
        services.AddScoped<IDbSession, DbSession>();
        services.AddSingleton<ITimeService, TimeService>();
        services.AddScoped<IOrderRepositoryService, OrderRepositoryService>();
    }
}